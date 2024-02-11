using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EasySaveConsole.Models
{
    class Log
    {
        [JsonProperty("Horodatage")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime Horodatage { get; set; }
        public string Name { get; set; }
        public string FileSource { get; set; }
        public string FileTarget { get; set; }
        public long FileSize { get; set; }
        public long FileTransferTime { get; set; }
    }
    public class CustomDateTimeConverter : IsoDateTimeConverter
    {
        public CustomDateTimeConverter()
        {
            DateTimeFormat = "dd/MM/yyyy HH:mm:ss";
        }
    }
}
