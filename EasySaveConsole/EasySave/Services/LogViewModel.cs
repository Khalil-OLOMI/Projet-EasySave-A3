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
    public class LogViewModel
    {
        private string log_file = "log.json";

        public LogViewModel() 
        {
            if (!File.Exists(log_file))
            {
                File.Create(log_file).Close();
                string json = JsonConvert.SerializeObject(new List<Log>(), Formatting.Indented);
                File.WriteAllText(log_file, json);
            }
        }

        public List<Log> GetLogs()
        {
            try
            {
                string jsonContent = File.ReadAllText(log_file);
                return JsonConvert.DeserializeObject<List<Log>>(jsonContent);
            }
            catch (Exception ex)
            {
                return new List<Log>(); // Or throw an exception or handle it according to your needs
            }
        }
        public void WriteLog(IBackup backup, long duration)
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
