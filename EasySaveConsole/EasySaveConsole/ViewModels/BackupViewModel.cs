using EasySaveConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySaveConsole.Services
{
    class BackupViewModel
    {
        public static void AddBackup(string type)
        {
            Console.WriteLine("Entrer le nom de la sauvegarde");
            string Name = Console.ReadLine();
            Console.WriteLine("Entrer l'adresse du repertoire source");
            string Source = Console.ReadLine();
            Console.WriteLine("Entrer l'adresse du repertoire cible");
            string Cible = Console.ReadLine();
            switch (type)
            {
                case "Complet":
                    IBackup completBackup = new CompletBackup();
                    completBackup.Name = Name;
                    completBackup.Source = Source;
                    completBackup.Cible = Cible;
                    Program.backupList.Add(completBackup);
                    break;
                case "Differential":
                    IBackup diffBackup = new DifferentialBackup();
                    diffBackup.Name = Name;
                    diffBackup.Source = Source;
                    diffBackup.Cible = Cible;
                    Program.backupList.Add(diffBackup);
                    break;
            }
        }

        public static void GetBackupList()
        {
            foreach (IBackup backup in Program.backupList)
            {
                Console.WriteLine("- " + backup.Name);
            }
        }
    }
}
