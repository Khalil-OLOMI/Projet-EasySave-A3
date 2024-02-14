using EasySave.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EasySave.Services
{
    class LogViewModel
    {
        static string log_file = Directory.GetCurrentDirectory() + "\\log\\log.json";

        public static void ReadLogFile()
        {
            try
            {
                // Vérifier si le fichier existe
                if (!File.Exists(log_file))
                {
                    Console.WriteLine("Le fichier journal spécifié n'existe pas.");
                    return;
                }

                // Lire le contenu du fichier
                string jsonContent = File.ReadAllText(log_file);

                // Désérialiser le contenu JSON en objets
                List<Log> logEntries = JsonConvert.DeserializeObject<List<Log>>(jsonContent);

                // Afficher les entrées du journal
                foreach (var entry in logEntries)
                {
                    Console.WriteLine($"Horodatage: {entry.Horodatage}");
                    Console.WriteLine($"Nom de sauvegarde: {entry.Name}");
                    Console.WriteLine($"Source: {entry.FileSource}");
                    Console.WriteLine($"Destination: {entry.FileTarget}");
                    Console.WriteLine($"Taille du fichier: {entry.FileSize}");
                    Console.WriteLine($"Temps de transfert: {entry.FileTransferTime}");
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Une erreur s'est produite lors de la lecture du fichier journal : {ex.Message}");
            }
        }
        public static void WriteLog(IBackup backup, long duration)
        {
            Log log = new Log
            {
                Horodatage = DateTime.Now,
                Name = backup.Name,
                FileSource = backup.Source,
                FileTarget = backup.Cible,
                FileSize = GetDirectorySize(backup.Source),
                FileTransferTime = duration
            };

            List<Log> logs = new List<Log>();

            // Vérifier si le fichier existe déjà
            if (File.Exists(log_file))
            {
                // Lire le contenu existant du fichier JSON dans une chaîne
                string json = File.ReadAllText(log_file);

                // Désérialiser la chaîne JSON en une liste d'objets
                logs = JsonConvert.DeserializeObject<List<Log>>(json);
            }

            // Ajouter le nouvel objet à la liste
            logs.Add(log);

            // Sérialiser la liste mise à jour en une chaîne JSON
            string updatedJson = JsonConvert.SerializeObject(logs, Formatting.Indented);

            // Écrire la chaîne JSON mise à jour dans le fichier
            File.WriteAllText(log_file, updatedJson);
        }
        static long GetDirectorySize(string directoryPath)
        {

            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            if (!directoryInfo.Exists)
            {
                return 0;
            }
            else
            {
                long directorySize = 0;

                // Add size of all files in the directory
                foreach (FileInfo file in directoryInfo.GetFiles("*", SearchOption.AllDirectories))
                {
                    directorySize += file.Length;
                }

                return directorySize;
            }


        }
    }
}
