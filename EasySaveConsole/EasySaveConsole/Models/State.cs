using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySaveConsole.Models
{
    class State
    {
        [JsonProperty("Horodatage")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime Horodatage { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string FileSource { get; set; }
        public string FileTarget { get; set; }
        public int TotalFilesToCopy { get; set; }
        public long TotalFilesSize { get; set; }
        public int NbFilesLeftToDo { get; set; }
        public double Progression { get; set; }

    }
    
}
