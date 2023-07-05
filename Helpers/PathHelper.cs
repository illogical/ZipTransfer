
namespace ZipTransfer.Helpers
{
    public static class PathHelper
    {
        public static string GetDestinationArchiveFilePath(string sourcePath, string destinationPath)
        {
            var dirInfo = new DirectoryInfo(sourcePath);
            string sourceFileName = string.Concat(dirInfo.Name, ".zip");

            string destinationFilePath = Path.Combine(destinationPath, sourceFileName);
            return destinationFilePath;
        }

        public static string GetDestinationVersionPath(string destinationPath) => Path.Combine(destinationPath, @"versions");
    }
}
