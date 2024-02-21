using EasySave.Helpers;
using EasySave.Models;
using EasySave.Services;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using RelayCommand = EasySave.Services.RelayCommand;

namespace EasySave.ViewModels;

public class BackupViewModel : ObservableObject
{
    private string backupFile = "backups.json";
    private DeepLTranslator translator; // Ajout de la classe de traduction

    public BackupViewModel()
    {
        InitBackup();

        Backups = GetBackups();
        PlayCommand = new RelayCommand(Play, CanPlay);
        DeleteCommand = new RelayCommand(Delete, CanDelete);
    }

    public ICommand PlayCommand { get; private set; }
    public ICommand DeleteCommand { get; private set; }


    private IBackup _selectedBackup;
    public IBackup SelectedBackup
    {
        get => _selectedBackup;
        set
        {
            if (_selectedBackup == value) return;

            _selectedBackup = value;
            OnPropertyChanged(nameof(SelectedBackup));
        }
    }

    private ObservableCollection<IBackup> _backups;
    public ObservableCollection<IBackup> Backups
    {
        get => _backups;
        set
        {
            if (_backups == value) return;

            _backups = value;
            OnPropertyChanged(nameof(Backups));
        }
    }

    public void InitBackup()
    {
        if (!File.Exists(backupFile))
        {
            string json = JsonConvert.SerializeObject(_backups, Formatting.Indented);
            File.WriteAllText(backupFile, json);
        }
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
        else if (SelectedBackup != null)
        {
            ExecuteBackup(SelectedBackup);

            MessageBox.Show("Backup finished.");
        }
    }

    private bool CanPlay(object parameter)
    {
        return SelectedBackup is not null;
    }

    private void Delete(object parameter)
    {
        if (SelectedBackup is not null)
        {
            DeleteBackup(SelectedBackup);
            MessageBox.Show("Backup deleted");
        }
    }

    private bool CanDelete(object parameter)
    {
        return SelectedBackup is not null;
    }

    private void SaveBackup()
    {
        string json = JsonConvert.SerializeObject(_backups, Formatting.Indented);
        File.WriteAllText(backupFile, json);
    }

    public void AddBackup(string Name, string Source, string Cible, string Type)
    {
        if (Source == Cible)
        {
            MessageBox.Show("Source identique à la destination.");
            return;
        }

        IBackup backup = Type.ToLower() switch
        {
            "complet" => new CompletBackup
            {
                Name = Name,
                Source = Source,
                Cible = Cible,
                Type = Type,
                Status = "Created"
            },
            "differential" => new DifferentialBackup
            {
                Name = Name,
                Source = Source,
                Cible = Cible,
                Type = Type,
                Status = "Created"
            },
            _ => throw new ArgumentException($"Invalid backup type ({Type})")
        };

        _backups.Add(backup);

        SaveBackup();

    }

    public void ExcuteBackups(ObservableCollection<IBackup> backupToExecute)
    {
        foreach (IBackup bkp in backupToExecute)
        {
            ExecuteBackup(bkp);
        }
    }

    public static void ExecuteBackup(IBackup backup)
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
        _backups.Remove(backup);
        SaveBackup();
    }
}