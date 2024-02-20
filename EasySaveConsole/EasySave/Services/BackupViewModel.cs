using EasySave.Helpers;
using EasySave.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;

namespace EasySave.Services
{
    public class BackupViewModel : INotifyPropertyChanged
    {
        private string backupFile = "backups.json";
        private DeepLTranslator translator; // Ajout de la classe de traduction
        private ObservableCollection<IBackup> backups;
        public ICommand PlayCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        private IBackup selectedBackup;
        public string BackupListHeaderText { get; set; }
        public string NameHeaderText { get; set; }
        public string TypeHeaderText { get; set; }
        public string StatusHeaderText { get; set; }
        public string ActionsHeaderText { get; set; }
        public string AddBackupButtonText { get; set; }
        public string PlayButtonText { get; set; }
        public string DeleteButtonText { get; set; }

        public IBackup SelectedBackup
        {
            get { return selectedBackup; }
            set
            {
                if (selectedBackup != value)
                {
                    selectedBackup = value;
                    OnPropertyChanged(nameof(SelectedBackup));
                }
            }
        }
        private async void TranslateTextElementsAsync()
        {
            try
            {
                // Translate text elements using the DeepLTranslator object
                BackupListHeaderText = await translator.TranslateAsync("Backup list");
                NameHeaderText = await translator.TranslateAsync("Name");
                TypeHeaderText = await translator.TranslateAsync("Type");
                StatusHeaderText = await translator.TranslateAsync("Status");
                ActionsHeaderText = await translator.TranslateAsync("Actions");
                AddBackupButtonText = await translator.TranslateAsync("Add backup");
                PlayButtonText = await translator.TranslateAsync("Play");
                DeleteButtonText = await translator.TranslateAsync("Delete");

                // Notify property changed for translated text properties
                OnPropertyChanged(nameof(BackupListHeaderText));
                OnPropertyChanged(nameof(NameHeaderText));
                OnPropertyChanged(nameof(TypeHeaderText));
                OnPropertyChanged(nameof(StatusHeaderText));
                OnPropertyChanged(nameof(ActionsHeaderText));
                OnPropertyChanged(nameof(AddBackupButtonText));
                OnPropertyChanged(nameof(PlayButtonText));
                OnPropertyChanged(nameof(DeleteButtonText));
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error translating text: {ex.Message}");
            }
        }
        public void InitBackup()
        {
            if (!File.Exists(backupFile))
            {
                File.Create(backupFile).Close();
                string json = JsonConvert.SerializeObject(backups, Formatting.Indented);
                File.WriteAllText(backupFile, json);
            }
        }
        public ObservableCollection<IBackup> Backups
        {
            get { return backups; }
            set
            {
                if (backups != value)
                {
                    backups = value;
                    OnPropertyChanged(nameof(Backups));
                }
            }
        }
        public BackupViewModel()
        {
            InitBackup();
            Backups = GetBackups();
            PlayCommand = new RelayCommand(Play, CanPlay);
            DeleteCommand = new RelayCommand(Delete, CanDelete);
            string apiKey = Config.ApiKey;
            translator = new DeepLTranslator(apiKey);

            // Translate text elements
            TranslateTextElementsAsync();
        }

        public ObservableCollection<IBackup> GetBackups()
        {
            string json = File.ReadAllText(backupFile);
            return JsonConvert.DeserializeObject<ObservableCollection<IBackup>>(json, new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new BackupConverter() }
            });
        }
        
        private void Play(object parameter)
        {
            ConfigViewModel configViewModel = new ConfigViewModel(Config.LoadConfig());
            string processName = configViewModel.ProcessName;

            if (Process.GetProcessesByName(processName).Length > 0)
            {
                MessageBox.Show($"Le processus {processName} est en cours d'exécution. Veuillez fermer toutes ses instances pour reprendre la sauvegarde.", "Processus en cours d'exécution", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            else
            {
                if (SelectedBackup != null)
                {
                    ExecuteBackup(SelectedBackup);
                    MessageBox.Show("Backup finished.", "Backup finished.", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        private bool CanPlay(object parameter)
        {
            return SelectedBackup != null;
        }
        private void Delete(object parameter)
        {
            if (SelectedBackup != null)
            {
                DeleteBackup(SelectedBackup);
                MessageBox.Show("Backup deleted", "Backup deleted", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private bool CanDelete(object parameter)
        {
            return SelectedBackup != null;
        }
        private void SaveBackup()
        {
            string json = JsonConvert.SerializeObject(backups, Formatting.Indented);
            File.WriteAllText(this.backupFile, json);
        }
        public void AddBackup(string Name, string Source, string Cible, string Type)
        {
            if (Source == Cible)
            {
                MessageBox.Show("Source identique à la destination.");
            }
            else
            {
                switch (Type.ToLower())
                {
                    case "complet":
                        IBackup completBackup = new CompletBackup();
                        completBackup.Name = Name;
                        completBackup.Source = Source;
                        completBackup.Cible = Cible;
                        completBackup.Type = Type;
                        completBackup.Status = "Created";
                        backups.Add(completBackup);
                        SaveBackup();
                        break;
                    case "differential":
                        IBackup diffBackup = new DifferentialBackup();
                        diffBackup.Name = Name;
                        diffBackup.Source = Source;
                        diffBackup.Cible = Cible;
                        diffBackup.Type = Type;
                        diffBackup.Status = "Created";
                        backups.Add(diffBackup);
                        SaveBackup();
                        break;
                    default:
                        break;
                }
            }

        }
        public void ExcuteBackups(ObservableCollection<IBackup> backupToExecute)
        {
            foreach (IBackup bkp in backupToExecute)
            {
                ExecuteBackup(bkp);
            }
        }
        public void ExecuteBackup(IBackup backup)
        {
            DateTime start = DateTime.Now;
            backup.Copy(backup.Source, backup.Cible);
            DateTime end = DateTime.Now;
            TimeSpan timeSpan = end - start;
            long duration = Convert.ToInt64(timeSpan.TotalSeconds);
            if (Config.LoadConfig().LogType == "XML")
            {
                new LogViewModel().WriteLogXml(backup, duration);
            }
            else
            {
                new LogViewModel().WriteJSONLog(backup, duration);
            }
        }
        public void DeleteBackup(IBackup backup)
        {
            backups.Remove(backup);
            SaveBackup();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}