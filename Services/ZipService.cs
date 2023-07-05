using System.Diagnostics;
using System.IO.Compression;
using ZipTransfer.Helpers;
using ZipTransfer.Models;

namespace ZipTransfer.Services
{
    public class ZipService
    {
        private LoggerService _logger;
        private FileSystemService _fileSystem;
        private VersionService _versionService;

        public ZipService(LoggerService logger, VersionService versionService)
        {
            _logger = logger;
            _fileSystem = new FileSystemService(logger);
            _versionService = versionService;
        }

        public void ZipAndMoveToDestination(string sourcePath, string destinationPath, string tempPath, bool deleteSourceFiles = false)
        {
            var archiveFilePath = ZipPath(sourcePath, tempPath); // returns the full path to the new zip file

            if (string.IsNullOrWhiteSpace(archiveFilePath))
            {
                _logger.WriteError($"Unable to create zip file to location: {archiveFilePath}");
                return;
            }

            if(deleteSourceFiles)
            {
                _fileSystem.DeleteFilesInPath(sourcePath);
            }

            var archiveFileName = new FileInfo(archiveFilePath).Name; // get the file name only to append it to the destinationPath
            _fileSystem.MoveFileToLocation(archiveFilePath, Path.Combine(destinationPath, archiveFileName));
            _fileSystem.DeletePathIfExists(archiveFilePath);

        }


        /// <summary>
        /// Uses the source (or subdirectories of the source) to zip each then copy the zip file(s) to the destinationPath.
        /// </summary>
        /// <param name="sourcePath">Parent directory. All of its child folders will become a Zip archive.</param>
        /// <param name="destinationPath">Location where the Zip files will be moved to.</param>
        /// <param name="tempPath">Path to store the zip files prior to moving them to the destination.</param>
        public void ZipConfiguredPathsAndMoveToDestination(List<Transfer> transfers, string tempPath)
        {
            foreach (var transfer in transfers)
            {
                if (!Directory.Exists(transfer.Source))
                {
                    _logger.WriteError($"Source Path is not valid: {transfer.Source}");
                    continue;
                }

                if (transfer.Versions.HasValue && transfer.Versions > 0)
                {
                    _versionService.CreateVersion(transfer.Source, transfer.Destination, transfer.Versions.Value);
                }


                if (transfer.ZipSubdirectories)
                {
                    var sourcePaths = new DirectoryInfo(transfer.Source).GetDirectories().Select(d => d.FullName);
                    foreach (string subDirPath in sourcePaths)
                    {
                        // TODO: delete as a separate pass after all successes?
                        ZipAndMoveToDestination(subDirPath, transfer.Destination, tempPath, transfer.DeleteAfterArchived);
                    }
                }
                else
                {
                    ZipAndMoveToDestination(transfer.Source, transfer.Destination, tempPath, transfer.DeleteAfterArchived);
                }
            }
        }

        /// <summary>
        /// Uses the sourcePath to zip each subdirectory and upon successful zip, copies the zip to the destinationPath.
        /// </summary>
        /// <param name="sourcePath">Parent directory. All of its child folders will become a Zip archive.</param>
        /// <param name="destinationPath">Location where the Zip files will be moved to.</param>
        public void ZipSubdirectoriesAndMoveToDestination(string sourcePath, string destinationPath, string tempPath)
        {
            if (!Directory.Exists(sourcePath))
            {
                _logger.WriteError($"Path is not valid: {sourcePath}");
                return;
            }

            var dirInfo = new DirectoryInfo(sourcePath);
            var subDirs = dirInfo.GetDirectories().Select(d => d.FullName);

            foreach (var subdir in subDirs)
            {
                ZipAndMoveToDestination(subdir, destinationPath, tempPath);
            }
        }

        private string? ZipPath(string sourcePath, string destinationPath)
        {
            if (!_fileSystem.ValidatePath(sourcePath) || !_fileSystem.ValidatePath(destinationPath))
            {
                return null;
            }

            Stopwatch zipTimer = new Stopwatch();
            _logger.WriteLine($"Zipping {sourcePath}...");

            // set destination filename based upon the source folder name
            string destinationFilePath = PathHelper.GetDestinationArchiveFilePath(sourcePath, destinationPath);//Path.Combine(tempDestinationPath, sourceFileName);

            // clean up existing Zip file if there was a prior failure
            //_fileSystem.DeleteIfExists(destinationFilePath);

            try
            {
                zipTimer.Start();
                ZipFile.CreateFromDirectory(sourcePath, destinationFilePath, CompressionLevel.Optimal, false);
                zipTimer.Stop();

                _logger.WriteLine($"Elapsed time: {_logger.FormatStopwatchOutput(zipTimer)} to zip to path: {destinationFilePath}");
                return destinationFilePath;
            }
            catch (Exception ex)
            {
                _logger.WriteError($"Error: Does {destinationFilePath} already exist? Unable to create new Zip file.");
                return null;
            }
        }

        

    }
}
