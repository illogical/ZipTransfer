
namespace ZipTransfer.Models
{
    public class Transfer
    {
        public string Source { get; set; }
        public string Destination { get; set; }
        /// <summary>
        /// When true, zip each subdirectory as a separate Zip file. When false, zip this directory as a single Zip file.
        /// </summary>
        public bool ZipSubdirectories { get; set; }     // true if each subdirectory should be zipped separately
        public bool DeleteAfterArchived { get; set; }   // Useful for archiving in-place
        public int? Versions { get; set; }

        public Transfer(string source, string destination, bool zipSubDirectories = false, bool deleteAfterArchived = false, int? versions = null)
        {
            Source = source;
            Destination = destination;
            ZipSubdirectories = zipSubDirectories;
            DeleteAfterArchived = deleteAfterArchived;
            Versions = versions;
        }
    }
}
