using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ZipTransfer.Services
{
    public class FileSystemService
    {
        private LoggerService _logger;
        public FileSystemService(LoggerService logger)
        {
            _logger = logger;
        }

        public void MoveArchiveToLocation(string sourceFilePath, string destinationFilePath)
        {
            FileInfo sourceFile = new FileInfo(sourceFilePath);

            try
            {
                sourceFile.CopyTo(destinationFilePath, true);
            }
            catch (Exception)
            {
                _logger.WriteError($"Error while copying file to {destinationFilePath}");
                throw;
            }

            try
            {
                _logger.WriteLine("Deleting temporary file...");
                sourceFile.Delete();
            }
            catch (Exception)
            {
                _logger.WriteError($"Error while deleting file {sourceFilePath}");
                throw;
            }
            _logger.WriteLine("Transfer complete.");
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
                _logger.WriteLine($"{filePath} already exists! Deleting to prepare for replacement...");
                File.Delete(filePath);
                _logger.WriteLine("Deletion complete.");
            }
            catch (Exception)
            {
                _logger.WriteError($"Error while deleting {filePath}");
            }

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
