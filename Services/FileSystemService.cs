using System.Diagnostics;
using System.Security.Cryptography;

namespace ZipTransfer.Services
{
    public class FileSystemService
    {
        private LoggerService _logger;
        public FileSystemService(LoggerService logger)
        {
            _logger = logger;
        }

        public void MoveFileToLocation(string sourceFilePath, string destinationFilePath)
        {
            Stopwatch transferTimer = new Stopwatch();
            transferTimer.Start();
            try
            {
                _logger.WriteLine($"Copying file to destination {destinationFilePath}");
                File.Copy(sourceFilePath, destinationFilePath, true);
                transferTimer.Stop();

                _logger.WriteLine($"Transfer complete in elapsed time: {_logger.FormatStopwatchOutput(transferTimer)}\n");
            }
            catch (Exception)
            {
                _logger.WriteError($"Error while copying file to {destinationFilePath}");
                throw;
            }
        }

        public void DeleteFilesInPath(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            try
            {
                _logger.WriteLine($"Deleting all files/directories in path: {filePath}...");

                foreach (var file in Directory.GetFiles(filePath))
                {
                    File.Delete(file);
                }

                foreach (var dir in Directory.GetDirectories(filePath))
                {
                    Directory.Delete(dir);
                }

                _logger.WriteLine("File/subdirectory deletion complete.");
            }
            catch (Exception ex)
            {
                _logger.WriteError($"Error while deleting files: {ex.Message}");
            }
        }

        public void DeletePathIfExists(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            try
            {
                _logger.WriteInfo($"{filePath} already exists! Deleting to prepare for replacement...");
                File.Delete(filePath);
                _logger.WriteInfo("Deletion complete.");
            }
            catch (Exception)
            {
                _logger.WriteError($"Error while deleting {filePath}");
            }

        }

        public bool ValidatePath(string path)
        {
            if (!Directory.Exists(path))
            {
                _logger.WriteError($"Error: {path} does not exist.");
                return false;
            }

            return true;
        }

        //https://stackoverflow.com/a/1359947/201115
        public static bool FilesAreEqual_Hash(FileInfo first, FileInfo second)
        {
            byte[] firstHash = MD5.Create().ComputeHash(first.OpenRead());
            byte[] secondHash = MD5.Create().ComputeHash(second.OpenRead());

            for (int i = 0; i < firstHash.Length; i++)
            {
                if (firstHash[i] != secondHash[i])
                    return false;
            }
            return true;
        }
    }
}
