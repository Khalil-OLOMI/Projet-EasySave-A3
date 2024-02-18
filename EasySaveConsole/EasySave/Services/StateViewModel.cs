using EasySave.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.Services
{
    public class StateViewModel
    {
        private string state_file = "state.json";

        public StateViewModel()
        {
            if (!File.Exists(state_file))
            {
                File.Create(state_file).Close();
                string json = JsonConvert.SerializeObject(new List<State>(), Formatting.Indented);
                File.WriteAllText(state_file, json);
            }
        }
        public List<State> ReadStateFile()
        {
            try
            {
                string jsonContent = File.ReadAllText(state_file);
                return JsonConvert.DeserializeObject<List<State>>(jsonContent);
            }
            catch (Exception ex)
            {
                return new List<State>();
            }
        }

        public void WriteState(State state)
        {
            List<State> states = new List<State>();
            // Vérifier si le fichier existe déjà
            string json = File.ReadAllText(state_file);
            // Désérialiser la chaîne JSON en une liste d'objets
            states = JsonConvert.DeserializeObject<List<State>>(json);
            // Ajouter le nouvel objet à la liste
            states.Add(state);

            // Sérialiser la liste mise à jour en une chaîne JSON
            string updatedJson = JsonConvert.SerializeObject(states, Formatting.Indented);

            // Écrire la chaîne JSON mise à jour dans le fichier
            File.WriteAllText(state_file, updatedJson);
        }
        public static long GetDirectorySize(string directoryPath)
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
        public static int FileNbre(string dossier)
        {
            int totalFichiers = 0;

            // Obtenez la liste des fichiers dans le dossier
            string[] fichiers = Directory.GetFiles(dossier);
            totalFichiers += fichiers.Length;

            // Obtenez la liste des sous-dossiers dans le dossier
            string[] sousDossiers = Directory.GetDirectories(dossier);

            // Parcourez récursivement les sous-dossiers pour compter les fichiers
            foreach (string sousDossier in sousDossiers)
            {
                totalFichiers += FileNbre(sousDossier);
            }

            return totalFichiers;
        }
    }
}
