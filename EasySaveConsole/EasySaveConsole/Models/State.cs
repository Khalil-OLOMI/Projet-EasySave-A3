using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySaveConsole.Models
{
    class State
    {
        public DateTime Horodatage { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string FileSource { get; set; }
        public string FileTarget { get; set; }
        public int TotalFilesToCopy { get; set; }
        public int TotalFilesSize { get; set; }
        public int NbFilesLeftToDo { get; set; }
        public int Progression { get; set; }

        public State(string name, DateTime horodatage) 
        {
            this.Name = name;
            this.Horodatage = horodatage;
        }

    }
}
