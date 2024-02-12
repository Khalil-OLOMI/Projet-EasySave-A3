using EasySaveConsole.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EasySaveConsole.Services
{
    class BackupViewModel
    {
        public static List<IBackup> backupList = new List<IBackup>();
        static DeepLTranslator translator; // Ajout de la classe de traduction

        public static async Task AddBackup(string type)
        {
            translator = new DeepLTranslator(Config.ApiKey); // Initialisation du traducteur

            Console.WriteLine(await translator.TranslateAsync("Enter backup name:")); // Entrer le nom de la sauvegarde
            string Name = Console.ReadLine();
            Console.WriteLine(await translator.TranslateAsync("Enter source directory address:")); // Entrer l'adresse du repertoire source
            string Source = Console.ReadLine();
            Console.WriteLine(await translator.TranslateAsync("Enter target directory address:")); // Entrer l'adresse du repertoire cible
            string Cible = Console.ReadLine();
            if(Source == Cible)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(await translator.TranslateAsync("Error :Target directory is the same as source directory"));
                Console.ResetColor();
            }
            else
            {
                switch (type) // JB: on pourrait avoir un type enum pour le type mais c'est du détail
                {
                    case "Complet":
                        IBackup completBackup = new CompletBackup();
                        completBackup.Name = Name;
                        completBackup.Source = Source;
                        completBackup.Cible = Cible;
                        backupList.Add(completBackup);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(await translator.TranslateAsync("Success : Backup created"));
                        Console.ResetColor();
                        break;
                    case "Differential":
                        IBackup diffBackup = new DifferentialBackup();
                        diffBackup.Name = Name;
                        diffBackup.Source = Source;
                        diffBackup.Cible = Cible;
                        backupList.Add(diffBackup);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(await translator.TranslateAsync("Success : Backup created"));
                        Console.ResetColor();
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(await translator.TranslateAsync("Incorrect choice.")); // Choix incorrect
                        Console.ResetColor();
                        break;
                }
            }
            
        }

        public static async Task GetBackupList()
        {
            translator = new DeepLTranslator(Config.ApiKey);
            Console.WriteLine(await translator.TranslateAsync("Backup list:")); // Backup list
            int nbre = 1;
            foreach (IBackup backup in backupList)
            {
                Console.WriteLine(nbre.ToString() + "- " + backup.Name);
                nbre++;
            }
        }

        public static async Task ExcuteBackups()
        {
            translator = new DeepLTranslator(Config.ApiKey);
            Console.WriteLine(await translator.TranslateAsync("Choose Backup:")); // Choose Backup:
            int nbre = 1;
            foreach (IBackup bkp in backupList)
            {
                Console.WriteLine(nbre.ToString() + "-" + bkp.Name);
                nbre++;
            }
            string input = Console.ReadLine();
            string[] segments = input.Split(';');
            foreach (var segment in segments)
            {
                // JB: ici le code est un peu difficile à lire, on peut ajouter des méthodes pour
                // Améliorer la lecture du code
                if (segment.Contains('-'))
                {
                    // Si l'utilisateur spécifie une plage de numéros (Ex: 1-3)
                    string[] range = segment.Split('-');
                    if (range.Length == 2 && int.TryParse(range[0], out int start) && int.TryParse(range[1], out int end))
                    {
                        for (int i = start; i <= end; i++)
                        {
                            if (i > 0 && i <= backupList.Count())
                            {
                                await ExecuteBackup(backupList[i - 1]);
                            }
                            else
                            {
                                Console.WriteLine(await translator.TranslateAsync($"Backup {i} does not exist.")); // La sauvegarde {i} n'existe pas.
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(await translator.TranslateAsync($"Invalid range format: {segment}")); // Format de plage invalide : {segment}
                    }
                }
                else
                {
                    // Si l'utilisateur spécifie un numéro de sauvegarde individuel
                    if (int.TryParse(segment, out int number))
                    {
                        if (number > 0 && number <= backupList.Count())
                        {
                            await ExecuteBackup(backupList[number - 1]);
                        }
                        else
                        {
                            Console.WriteLine(await translator.TranslateAsync($"Backup {number} does not exist.")); // La sauvegarde {number} n'existe pas.
                        }
                    }
                    else
                    {
                        Console.WriteLine(await translator.TranslateAsync($"Invalid format: {segment}")); // Format invalide : {segment}
                    }
                }
            }
        }

        static async Task ExecuteBackup(IBackup backup)
        {
            translator = new DeepLTranslator(Config.ApiKey);
            Console.WriteLine(await translator.TranslateAsync("Backup in progress...")); // Backup in progress...
            DateTime start = DateTime.Now;
            backup.Copy(backup.Source, backup.Cible);
            DateTime end = DateTime.Now;
            TimeSpan timeSpan = end - start;
            long duration = Convert.ToInt64(timeSpan.TotalSeconds);
            LogViewModel.WriteLog(backup, duration);
            Console.WriteLine();
            Console.WriteLine(await translator.TranslateAsync("Backup completed.")); // Backup completed.
        }

        public static async Task DeleteBackup()
        {
            translator = new DeepLTranslator(Config.ApiKey);
            Console.WriteLine(await translator.TranslateAsync("Choose Backup:")); // Choose Backup:
            int nbre = 1;
            foreach (IBackup bkp in backupList)
            {
                Console.WriteLine(nbre.ToString() + "-" + bkp.Name);
                nbre++;
            }
            int choice = Convert.ToInt32(Console.ReadLine());
            IBackup backup = backupList[choice - 1];
            backupList.Remove(backup);

        }
    }
}