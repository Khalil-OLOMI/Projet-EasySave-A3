using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySaveConsole.Models
{
    class Sauvegarde
    {
        public string Name { get; set; }
        public string Source { get; set; }
        public string Cible { get; set; }
        public string Type { get; set; }

        public Sauvegarde(string name, string source, string cible, string type)
        {
            this.Name = name;
            this.Source = source;
            this.Cible = cible;
            this.Type = type;
        }
    }
}
