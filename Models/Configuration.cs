
namespace ZipTransfer.Models
{
    public class Configuration
    {
        /// <summary>
        /// List of source paths that will be zipped.
        /// </summary>
        public List<Transfer> Transfers { get; set; }

        /// <summary>
        /// After all zip files are created, move them to their final location
        /// </summary>

        public string TempLocation { get; set; }
    }
}
