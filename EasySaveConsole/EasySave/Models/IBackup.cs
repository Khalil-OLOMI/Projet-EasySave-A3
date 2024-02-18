using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EasySave.Models
{
    public interface IBackup
    {
        string Name { get; set; }
        string Source { get; set; }
        string Cible { get; set; }
        string Type { get; set; }
        string Status { get; set; }
        void Copy(string source, string cible);
    }
}
