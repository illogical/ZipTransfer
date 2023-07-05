using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipTransfer.Models
{
    public class Transfer
    {
        public string Source { get; set; }
        public string Destination { get; set; }
        /// <summary>
        /// When true, zip each subdirectory as a separate Zip file. When false, zip this directory as a single Zip file.
        /// </summary>
        public bool ZipSubdirectories { get; set; }
        public bool DeleteAfterArchived { get; set; }   // Useful for archiving in-place
    }
}
