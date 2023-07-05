using ZipTransfer.Helpers;
using ZipTransfer.Models;

namespace ZipTransfer.Services
{
    public class VersionService
    {
        private LoggerService _logger;

        public VersionService(LoggerService logger)
        {
            _logger = logger;
        }

        public void CreateVersion(Transfer transfer)
        {
            if (!transfer.Versions.HasValue || transfer.Versions == 0)
            {
                return;
            }

            string destinationFilePath = PathHelper.GetDestinationArchiveFilePath(transfer.Source, transfer.Destination);

            if(!File.Exists(destinationFilePath))
            {
                // this is the first time the file has been archived, so there are no versions to create
                return;
            }

            string versionDirectoryPath = PathHelper.GetDestinationVersionPath(transfer.Destination);

            if (!Directory.Exists(versionDirectoryPath))
            {
                _logger.WriteLine($"Creating version directory: {versionDirectoryPath}");
                Directory.CreateDirectory(versionDirectoryPath);
            }

            // check if the .zip file count is greater than the number of versions to keep
            var versionFiles = Directory.GetFiles(versionDirectoryPath, "*.zip");
            string newVersionFilePath = GetNewVersionFilePath(transfer.Destination, transfer.Source);

            if (File.Exists(newVersionFilePath))
            {
                _logger.WriteError($"Error: {newVersionFilePath} version already exists. Skipping this version.");
                return;
            }

            // Copy the existing file to the version directory and append the date/time to the file name

            _logger.WriteLine($"Creating version: {newVersionFilePath}");
            File.Copy(destinationFilePath, newVersionFilePath, false);

            if (versionFiles.Length >= transfer.Versions.Value)
            {
                DeleteOldestVersion(versionFiles);
            }
        }

        private string GetNewVersionFilePath(string destinationPath, string sourcePath)
        {
            string versionDirectoryPath = PathHelper.GetDestinationVersionPath(destinationPath);
            string destinationFileNameWithoutExtension = Path.GetFileName(sourcePath);
            string formattedDateTime = DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss");

            string newFilePath = Path.Combine(versionDirectoryPath, $"{destinationFileNameWithoutExtension}_{formattedDateTime}.zip");

            return newFilePath;
        }


        private void DeleteOldestVersion(string[] versionFilePaths)
        {
            // TODO: this currently compares the file creation time. Should it compare the file name instead?
            string oldestFile = versionFilePaths[0];

            if (versionFilePaths.Length > 1)
            {
                for (int i = 1; i < versionFilePaths.Length; i++)
                {
                    if (File.GetCreationTime(oldestFile) > File.GetCreationTime(versionFilePaths[i]))
                    {
                        oldestFile = versionFilePaths[i];
                    }
                }
            }

            _logger.WriteLine($"Deleting oldest version: {oldestFile}");
            File.Delete(oldestFile);
        }
    }
}
