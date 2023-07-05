using System;
using System.Collections.Generic;
using System.IO.Compression;
using ZipTransfer.Models;

namespace ZipTransfer.Services
{
    public class ZipService
    {
        // TODO: keep the last x number of zip files with a naming convention. Currently overwrites each time.

        private LoggerService _logger;
        private FileSystemService _fileSystem;

        public ZipService(LoggerService logger)
        {
            _logger = logger;
            _fileSystem = new FileSystemService(logger);
        }

        public void ZipAndMoveToDestination(string sourcePath, string destinationPath, string tempPath)
        {
            var archiveFilePath = ZipPath(sourcePath, tempPath); // returns the full path to the new zip file

            if (string.IsNullOrWhiteSpace(archiveFilePath))
            {
                _logger.WriteError($"Unable to create zip file to location: {archiveFilePath}");
                return;
            }

            var archiveFileName = new FileInfo(archiveFilePath).Name; // get the file name only to append it to the destinationPath
            _fileSystem.MoveArchiveToLocation(archiveFilePath, Path.Combine(destinationPath, archiveFileName));

        }

        public void ZipAndDeleteOriginalsAndMoveToDestination(string sourcePath, string destinationPath, string tempPath)
        {
            var archiveFilePath = ZipPath(sourcePath, tempPath);

            if (string.IsNullOrWhiteSpace(archiveFilePath))
            {
                _logger.WriteError($"Unable to create zip file to location: {archiveFilePath}");
                return;
            }

            var archiveFileName = new FileInfo(archiveFilePath).Name; // get the file name only to append it to the destinationPath
            _fileSystem.DeleteFilesInPath(sourcePath);
            _fileSystem.MoveArchiveToLocation(archiveFilePath, Path.Combine(destinationPath, archiveFileName));
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

                if (transfer.ZipSubdirectories)
                {
                    var sourcePaths = new DirectoryInfo(transfer.Source).GetDirectories().Select(d => d.FullName);
                    foreach (string subDirPath in sourcePaths)
                    {
                        if (transfer.DeleteAfterArchived)
                        {
                            ZipAndDeleteOriginalsAndMoveToDestination(subDirPath, transfer.Destination, tempPath);
                        }
                        else
                        {
                            ZipAndMoveToDestination(subDirPath, transfer.Destination, tempPath);
                        }
                    }
                }
                else
                {
                    if (transfer.DeleteAfterArchived)
                    {
                        ZipAndDeleteOriginalsAndMoveToDestination(transfer.Source, transfer.Destination, tempPath);
                    }
                    else
                    {
                        ZipAndMoveToDestination(transfer.Source, transfer.Destination, tempPath);
                    }

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

        private string? ZipPath(string sourcePath, string tempDestinationPath)
        {
            if (!ValidatePath(sourcePath) || !ValidatePath(tempDestinationPath))
            {
                return null;
            }

            _logger.WriteLine($"Zipping {sourcePath}...");

            // set destination filename based upon the source folder name
            var dirInfo = new DirectoryInfo(sourcePath);
            string sourceFileName = string.Concat(dirInfo.Name, ".zip");
            string destinationFilePath = Path.Combine(tempDestinationPath, sourceFileName);

            // clean up existing Zip file if there was a prior failure
            //_fileSystem.DeleteIfExists(destinationFilePath);

            try
            {
                ZipFile.CreateFromDirectory(sourcePath, destinationFilePath, CompressionLevel.Optimal, false);
                _logger.WriteLine($"Path successfully zipped to {destinationFilePath}");
                return destinationFilePath;
            }
            catch (Exception ex)
            {
                _logger.WriteError($"Error: Does {destinationFilePath} already exist? Unable to create new Zip file.");
                return null;
            }
        }

        private bool ValidatePath(string path)
        {
            if (!Directory.Exists(path))
            {
                _logger.WriteLine($"Error: {path} does not exist.");
                return false;
            }

            return true;
        }

    }
}
