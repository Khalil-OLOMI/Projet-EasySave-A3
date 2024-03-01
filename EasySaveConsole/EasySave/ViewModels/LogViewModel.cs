using EasySave.Models;
using Newtonsoft.Json;
using System.IO;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using EasySave.Helpers;

namespace EasySave.ViewModels
{
    public class LogViewModel : ObservableObject
    {
        private readonly string log_file = "log.json";
        private readonly string filePath = "log.xml";
        private DeepLTranslator translator; // Ajout de la classe de traduction
        public string FIlesizeText { get; set; }
        public string DateHeaderText { get; set; }
        public string LogHeaderText { get; set; }
        public string TimeText { get; set; }
        public string NameText { get; set; }

        public ObservableCollection<Log> Logs { get; set; }
        private Config Config { get; set; }

        public LogViewModel()
        {
            if (!File.Exists(log_file))
            {
                File.Create(log_file).Close();
                string json = JsonConvert.SerializeObject(new ObservableCollection<Log>(), Formatting.Indented);
                File.WriteAllText(log_file, json);
            }

            if (!File.Exists(filePath))
            {
                File.Create(log_file).Close();
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(ObservableCollection<Log>));

                using StreamWriter writer = new StreamWriter(filePath);
                xmlSerializer.Serialize(writer, new ObservableCollection<Log>());
            }

            if (Config.LoadConfig().LogType == "XML")
            {
                Logs = ReadXMLLog();
            }
            else
            {
                Logs = GetJSONLogs();
            }

            string apiKey = Config.ApiKey;
            translator = new DeepLTranslator(apiKey);

            // Translate text elements
            TranslateTextElementsAsync();
        }

        private async void TranslateTextElementsAsync()
        {
            try
            {
                // Translate text elements using the DeepLTranslator object
                LogHeaderText = await translator.TranslateAsync("Logs");
                DateHeaderText = await translator.TranslateAsync("Execution date");
                NameText = await translator.TranslateAsync("Name");
                FIlesizeText = await translator.TranslateAsync("File size in (KO)");
                TimeText = await translator.TranslateAsync("Time (ms)");


                // Notify property changed for translated text properties
                OnPropertyChanged(nameof(LogHeaderText));
                OnPropertyChanged(nameof(DateHeaderText));
                OnPropertyChanged(nameof(NameText));
                OnPropertyChanged(nameof(FIlesizeText));
                OnPropertyChanged(nameof(TimeText));

            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error translating text: {ex.Message}");
            }
        }

        public ObservableCollection<Log> GetJSONLogs()
        {
            try
            {
                string jsonContent = File.ReadAllText(log_file);
                return JsonConvert.DeserializeObject<ObservableCollection<Log>>(jsonContent);
            }
            catch (Exception ex)
            {
                return new ObservableCollection<Log>(); // Or throw an exception or handle it according to your needs
            }
        }

        public void WriteJSONLog(IBackup backup, long duration)
        {
            Log log = new()
            {
                Horodatage = DateTime.Now,
                Name = backup.Name,
                FileSource = backup.Source,
                FileTarget = backup.Cible,
                FileSize = GetDirectorySize(backup.Source),
                FileTransferTime = duration,
                FileEncryptTime = backup.EncryptTime,
            };

            List<Log> logs = new();

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

        private static long GetDirectorySize(string directoryPath)
        {

            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            if (!directoryInfo.Exists)
            {
                return 0;
            }


            long directorySize = 0;

            // Add size of all files in the directory
            foreach (FileInfo file in directoryInfo.GetFiles("*", SearchOption.AllDirectories))
            {
                directorySize += file.Length;
            }

            return directorySize;
        }
        public ObservableCollection<Log> ReadXMLLog()
        {
            try
            {
                // Lire le contenu du fichier XML dans une chaîne
                string xml = File.ReadAllText(filePath);

                // Désérialiser la chaîne XML en une liste d'objets Log
                XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Log>));

                using StringReader reader = new StringReader(xml);
                return (ObservableCollection<Log>)serializer.Deserialize(reader);
            }
            catch (Exception ex)
            {
                return new ObservableCollection<Log>();
            }
        }

        public void WriteLogXml(IBackup backup, long duration)
        {
            // Créer un nouvel objet Log
            Log log = new Log
            {
                Horodatage = DateTime.Now,
                Name = backup.Name,
                FileSource = backup.Source,
                FileTarget = backup.Cible,
                FileSize = GetDirectorySize(backup.Source),
                FileTransferTime = duration
            };

            // Liste pour stocker les logs
            List<Log> logs = new List<Log>();

            // Vérifier si le fichier journal existe déjà
            if (File.Exists(filePath))
            {
                // Lire le contenu existant du fichier XML dans une chaîne
                string xml = File.ReadAllText(filePath);

                // Désérialiser la chaîne XML en une liste d'objets Log
                XmlSerializer serializer = new XmlSerializer(typeof(List<Log>));
                using StringReader reader = new StringReader(xml);
                logs = (List<Log>)serializer.Deserialize(reader);
            }
            else
            {
                // Créer le fichier s'il n'existe pas
                File.Create(filePath).Close();
            }

            // Ajouter le nouvel objet à la liste
            logs.Add(log);

            // Sérialiser la liste mise à jour en une chaîne XML
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Log>));
            using StreamWriter writer = new StreamWriter(filePath);

            xmlSerializer.Serialize(writer, logs);
        }
    }
}
