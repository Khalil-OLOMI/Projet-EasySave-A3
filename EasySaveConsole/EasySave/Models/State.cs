using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.Models
{
    // JB: public?
    public class State
    {
        [JsonProperty("Horodatage")]
        [JsonConverter(typeof(CustomDateTimeConverter))]

        //private DateTime _Horodatage;
        //private string _Name;
        //private string _Status;
        //private string _FileSource;
        //private string _FileTarget;
        //private int _TotalFilesToCopy;
        //private long _TotalFilesSize;
        //private int _NbFilesLeftToDo;
        //private double _Progression;


        //public DateTime Horodatage
        //{
        //    get => _Horodatage;
        //    set => _Horodatage = value;
        //}
        //
        //public string Name
        //{
        //    get => _Name;
        //    set => _Name = value;
        //}
        //
        //public string Status
        //{
        //    get => _Status;
        //    set => _Status = value;
        //}
        //
        //public string FileSource
        //{
        //    get => _FileSource;
        //    set => _FileSource = value;
        //}
        //
        //public string FileTarget
        //{
        //    get => _FileTarget;
        //    set => _FileTarget = value;
        //}
        //
        //public int TotalFilesToCopy
        //{
        //    get => _TotalFilesToCopy;
        //    set => _TotalFilesToCopy = value;
        //}
        //public long TotalFilesSize
        //{
        //    get => _TotalFilesSize;
        //    set => _TotalFilesSize = value;
        //}
        //
        //public int NbFilesLeftToDo
        //{
        //    get => _NbFilesLeftToDo;
        //    set => _NbFilesLeftToDo = value;
        //}
        //
        //
        //public double Progression
        //{
        //    get => _Progression;
        //    set => _Progression = value;
        //}


        
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
