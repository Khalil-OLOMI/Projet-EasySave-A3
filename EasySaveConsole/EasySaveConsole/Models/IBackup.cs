using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySaveConsole.Models
{
    interface IBackup
    {
        string Name { get; set; }
        string Source { get; set; }
        string Cible { get; set; }
        void Copy(string source, string cible);
    }
}
