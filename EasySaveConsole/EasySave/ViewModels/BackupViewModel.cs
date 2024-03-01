using EasySave.Helpers;
using EasySave.Models;
using EasySave.Services;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RelayCommand = EasySave.Services.RelayCommand;

namespace EasySave.ViewModels;

public class BackupViewModel : ObservableObject
{
    private string backupFile = "backups.json";
    private DeepLTranslator translator; // Ajout de la classe de traduction

    public string BackupListHeaderText { get; set; }
    public string NameHeaderText { get; set; }
    public string TypeHeaderText { get; set; }
    public string StatusHeaderText { get; set; }
    public string ActionsHeaderText { get; set; }
    public string AddBackupButtonText { get; set; }
    public string PlayButtonText { get; set; }
    public string DeleteButtonText { get; set; }
    public string SrcPath { get; set; }
    public string Browse { get; set; }
    public string Save { get; set; }
    public string Cible { get; set; }


    private bool _isComplete;
    public bool IsComplete
    {
        get { return _isComplete; }
        set
        {
            if (_isComplete != value)
            {
                _isComplete = value;
                OnPropertyChanged(nameof(IsComplete));
            }
        }
    }

    private bool _isDifferential;
    public bool IsDifferential
    {
        get { return _isDifferential; }
        set
        {
            if (_isDifferential != value)
            {
                _isDifferential = value;
                OnPropertyChanged(nameof(IsDifferential));
            }
        }
    }


    public BackupViewModel()
    {
        InitBackup();

        Backups = GetBackups();

        PlayCommand = new RelayCommand(Play, CanPlay);
        DeleteCommand = new RelayCommand(Delete, CanDelete);
        PauseCommand = new RelayCommand(Pause, CanPause);
        StopCommand = new RelayCommand(Stop, CanStop);
        ResumeCommand = new RelayCommand(Resume, CanResume);

        string apiKey = Config.ApiKey;
        translator = new DeepLTranslator(apiKey);

        // Translate text elements
        var task = Task.Run(() => TranslateTextElementsAsync());
        task.Wait();
    }


    private async Task TranslateTextElementsAsync()
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
            SrcPath = await translator.TranslateAsync("Fichiers à sauvegarder:");
            Browse = await translator.TranslateAsync("Browse");
            Save = await translator.TranslateAsync("Sauvegarder");
            Cible = await translator.TranslateAsync("Sauvegarder ici:");
            // Notify property changed for translated text properties
            OnPropertyChanged(nameof(BackupListHeaderText));
            OnPropertyChanged(nameof(NameHeaderText));
            OnPropertyChanged(nameof(TypeHeaderText));
            OnPropertyChanged(nameof(StatusHeaderText));
            OnPropertyChanged(nameof(ActionsHeaderText));
            OnPropertyChanged(nameof(AddBackupButtonText));
            OnPropertyChanged(nameof(PlayButtonText));
            OnPropertyChanged(nameof(DeleteButtonText));
            OnPropertyChanged(nameof(SrcPath));
            OnPropertyChanged(nameof(Browse));
            OnPropertyChanged(nameof(Save));
            OnPropertyChanged(nameof(Cible));
        }
        catch (Exception ex)
        {

            Console.WriteLine($"Error translating text: {ex.Message}");
        }
    }

    public ICommand PlayCommand { get; private set; }
    public ICommand DeleteCommand { get; private set; }
   
    public ICommand PauseCommand { get; private set; }
    public ICommand StopCommand { get; private set; }
    public ICommand ResumeCommand { get; private set; }



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

    private bool CanPlay(object parameter)
    {
        return !IsBackupRunning && SelectedBackup != null;
    }

    private void Pause(object parameter)
    {
        if (SelectedBackup != null)
        {
            if (SelectedBackup.IsPaused())
            {
                SelectedBackup.Resume();
            }
            else
            {
                ChangeBackupStatus(SelectedBackup, "Pause");
                SelectedBackup.Pause();
            }
        }
    }

    private bool CanPause(object parameter)
    {
        return SelectedBackup != null && !SelectedBackup.IsPaused(); // Allow pausing if the backup is not already paused
    }

   

    private void Stop(object parameter)
    {
        if (SelectedBackup != null)
        {
            ChangeBackupStatus(SelectedBackup, "Terminer");
            SelectedBackup.Stop();
        }
    }

    private bool CanStop(object parameter)
    {
        return SelectedBackup != null && !SelectedBackup.IsPaused(); // Allow stopping if the backup is not paused
    }


    private void Resume(object parameter)
    {
        if (SelectedBackup != null)
        {
            ChangeBackupStatus(SelectedBackup, "Active");
            SelectedBackup.Resume();
        }
    }

    private bool CanResume(object parameter)
    {
        return SelectedBackup != null && SelectedBackup.IsPaused();
    }


    private bool _isBackupRunning;
    public bool IsBackupRunning
    {
        get { return _isBackupRunning; }
        set
        {
            if (_isBackupRunning != value)
            {
                _isBackupRunning = value;
                OnPropertyChanged(nameof(IsBackupRunning));
                CommandManager.InvalidateRequerySuggested(); // Ensures that the commands are re-evaluated
            }
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

    private async void Play(object parameter)
    {
        ConfigViewModel configViewModel = new ConfigViewModel(Config.LoadConfig());
        string processName = configViewModel.ProcessName;

        if (Process.GetProcessesByName(processName).Length > 0)
        {
            MessageBox.Show($"Le processus {processName} est en cours d'exécution. Veuillez fermer toutes ses instances pour reprendre la sauvegarde.", "Processus en cours d'exécution", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else if (SelectedBackup != null)
        {
            if (!SelectedBackup.IsPaused()) 
            {
                SelectedBackup.NbreFile = 0;
                await ChangeBackupStatus(SelectedBackup, "Active" );

                await ExecuteBackupAsync(SelectedBackup);
                //SelectedBackup.Stop(); // Deactivate the Play button after backup completion

                await ChangeBackupStatus(SelectedBackup, "Terminer");
                
                MessageBox.Show("Backup finished.");
            }
            else
            {
                MessageBox.Show("Backup is already running.");
            }
        }
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

    public void AddBackup(string Name, string Source, string Cible, bool isComplete, bool isDifferential)
    {
        if (Source == Cible)
        {
            MessageBox.Show("Source identique à la destination.");
            return;
        }
        string Type= "Complet";
        if (!isComplete)
        {
            Type = "Differential";
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

    //public void ExcuteBackups(ObservableCollection<IBackup> backupToExecute)
    //{
    //    foreach (IBackup bkp in backupToExecute)
    //    {
    //        ExecuteBackup(bkp);
    //    }
    //}

    public async Task ExcuteBackupsAsync(ObservableCollection<IBackup> backupsToExecute)
    {
        try
        {
            // Create tasks for each backup operation
            var backupTasks = backupsToExecute.Select(bkp => Task.Run(async () =>
            {
                try
                {
                    await ExecuteBackupAsync(bkp);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error executing backup {bkp.Name}: {ex.Message}");
                }
            }));

            // Wait for all backup tasks to complete
            await Task.WhenAll(backupTasks);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing backups: {ex.Message}");
        }
    }


    private async Task ExecuteBackupAsync(IBackup backup)
    {
        DateTime start = DateTime.Now;

        await Task.Run(() => backup.Copy(backup.Source, backup.Cible));

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

    private async Task ChangeBackupStatus(IBackup backup, string status)
    {
        var backupToUpdate = _backups.FirstOrDefault(b => b == backup);

        if (backupToUpdate != null)
        {
            backupToUpdate.Status = status;
            SaveBackup();
            OnPropertyChanged(nameof(Backups));
        }
    }

    private async Task InitializeProgress(IBackup backup)
    {
        var backupToUpdate = _backups.FirstOrDefault(b => b == backup);

        if (backupToUpdate != null)
        {
            backupToUpdate.Progression = 0;
            SaveBackup();
            OnPropertyChanged(nameof(Backups));
        }
    }

}