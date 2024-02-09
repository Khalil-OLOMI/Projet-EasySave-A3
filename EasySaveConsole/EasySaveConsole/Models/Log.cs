using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySaveConsole.Models
{
    class Log
    {
        public DateTime Horodatage { get; set; }
        public string Name { get; set; }
        public string FileSource { get; set; }
        public string FileTarget { get; set; }
        public int FileSize { get; set; }
        public float FileTransferTime { get; set; }
    }
}
