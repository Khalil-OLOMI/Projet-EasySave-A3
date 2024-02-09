using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySaveConsole.Models
{
    internal class DifferentialBackup : IBackup
    {
        public string Name { get; set; }
        public string Source { get; set; }
        public string Cible { get; set; }
        public void Copy()
        {
            
        }
    }
}
