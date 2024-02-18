using EasySave.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
            Backups  = GetBackups();
            PlayCommand = new RelayCommand(Play, CanPlay);
            DeleteCommand = new RelayCommand(Delete, CanDelete);
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
            if (SelectedBackup != null)
            {
                ExecuteBackup(SelectedBackup);
                MessageBox.Show("Backup finish");
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
                MessageBox.Show("Backup deleted");
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
            if(Source == Cible)
            {
                MessageBox.Show("Source identique à la destination.");
            }
            else
            {
                switch (Type) 
                {
                    case "Complet":
                        IBackup completBackup = new CompletBackup();
                        completBackup.Name = Name;
                        completBackup.Source = Source;
                        completBackup.Cible = Cible;
                        backups.Add(completBackup);
                        SaveBackup();
                        break;
                    case "Differential":
                        IBackup diffBackup = new DifferentialBackup();
                        diffBackup.Name = Name;
                        diffBackup.Source = Source;
                        diffBackup.Cible = Cible;
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
            new LogViewModel().WriteLog(backup, duration);
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