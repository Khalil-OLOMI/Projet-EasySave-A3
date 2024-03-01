using RelayCommandSET = EasySave.Helpers.RelayCommand;
using System.Collections.ObjectModel;
using System.Windows.Input;
using EasySave.Helpers;

namespace EasySave.ViewModels;

public class ConfigViewModel : ObservableObject
{
    private Config _config;
    private readonly DeepLTranslator _translator;


    public ObservableCollection<string> FichierPrioritaires
    {
        get => new(_config.FichierPrioritaires);
        set
        {
            _config.FichierPrioritaires = value.ToList();
            OnPropertyChanged(nameof(FichierPrioritaires));
        }
    }

    private string _newFichierPrioritaireToAdd;
    public string NewFichierPrioritaireToAdd
    {
        get => _newFichierPrioritaireToAdd;
        set
        {
            if (_newFichierPrioritaireToAdd == value) return;

            _newFichierPrioritaireToAdd = value;
            OnPropertyChanged(nameof(NewFichierPrioritaireToAdd));
        }
    }


    private void AddFichierPrioritaire()
    {
        if (string.IsNullOrEmpty(NewFichierPrioritaireToAdd))
        {
            return;
        }

        _config.FichierPrioritaires.Add(NewFichierPrioritaireToAdd);

        FichierPrioritaires = new ObservableCollection<string>(_config.FichierPrioritaires);
        NewFichierPrioritaireToAdd = string.Empty;
    }




    public ConfigViewModel(Config config)
    {
        Config.LoadConfig();
        // Initialize DeepLTranslator with the API key
        string apiKey = Config.ApiKey;
        _config = config;
        _translator = new DeepLTranslator(apiKey);

        // Initialize the selected log type based on the value of LogType
        SelectedLogType = _config.LogType;

        // Initialize the selected language based on the value of TargetLanguage
        SelectedLanguage = GetLanguageFromCode(_config.TargetLanguage);
        ProcessName = _config.ProcessName;

        // Initialize encrypted file extensions
        EncryptedFileExtensions = new ObservableCollection<string>(_config.EncryptedFileExtensions);

        SaveCommand = new RelayCommandSET(SaveConfig);
        AddExtensionCommand = new RelayCommandSET(AddExtension);
        AddFichierPrioritaireCommand = new RelayCommandSET(AddFichierPrioritaire);
        TranslateText();
    }

    private string _SettingText;
    public string SettingText
    {
        get { return _SettingText; }
        set { _SettingText = value; OnPropertyChanged(SettingText); }
    }


    private string _GESText;
    public string GESText
    {
        get { return _GESText; }
        set { _GESText = value; OnPropertyChanged(GESText); }
    }

    private string _targetLanguageText;
    public string TargetLanguageText
    {
        get => _targetLanguageText;
        set 
        {
            if (_targetLanguageText == value) return;

            _targetLanguageText = value;
            OnPropertyChanged(nameof(TargetLanguageText));
        }
    }
    private string _extensionText;
    public string Extension
    {
        get => _extensionText;
        set
        {
            if (_extensionText == value) return;

            _extensionText = value;
            OnPropertyChanged(nameof(Extension));
        }
    }

    private string _logiText;
    public string Logi
    {
        get => _logiText;
        set
        {
            if (_logiText == value) return;

            _logiText = value;
            OnPropertyChanged(nameof(Logi));
        }
    }

    private string _logtypeText;
    public string Logtype
    {
        get => _logtypeText;
        set
        {
            if (_logtypeText == value) return;

            _logtypeText = value;
            OnPropertyChanged(nameof(Logtype));
        }
    }

    private string _addText;
    public string AddText
    {
        get => _addText;
        set
        {
            if (_addText == value) return;

            _addText = value;
            OnPropertyChanged(nameof(AddText));
        }
    }

    private string _savetext;
    public string SaveText
    {
        get => _savetext;
        set
        {
            if (_savetext == value) return;

            _savetext = value;
            OnPropertyChanged(nameof(SaveText));
        }
    }

    private bool _isEnglish;
    public bool IsEnglish
    {
        get => _isEnglish;
        set
        {
            if (_isEnglish == value) return;

            _isEnglish = value;
            if (_isEnglish)
            {
                IsFrench = false;
                IsSpanish = false;
                SelectedLanguage = "English";
            }
            OnPropertyChanged(nameof(IsEnglish));
        }
    }

    private bool _isFrench;
    public bool IsFrench
    {
        get => _isFrench;
        set
        {
            if (_isFrench == value) return;

            _isFrench = value;
            if (_isFrench)
            {
                IsEnglish = false;
                IsSpanish = false;
                SelectedLanguage = "French";
            }

            OnPropertyChanged(nameof(IsFrench));
        }
    }

    private bool _isSpanish;
    public bool IsSpanish
    {
        get => _isSpanish;
        set
        {
            if (_isSpanish == value) return;

            _isSpanish = value;
            if (_isSpanish)
            {
                IsEnglish = false;
                IsFrench = false;
                SelectedLanguage = "Spanish";
            }
            OnPropertyChanged(nameof(IsSpanish));
        }
    }

    private string _selectedLanguage;
    public string SelectedLanguage
    {
        get => _selectedLanguage;
        set
        {
            if (_selectedLanguage == value) return;

            _selectedLanguage = value;
            OnPropertyChanged(nameof(SelectedLanguage));
            SaveSelectedLanguage(value);
        }
    }

    public ObservableCollection<string> EncryptedFileExtensions
    {
        get => new(_config.EncryptedFileExtensions);
        set
        {
            _config.EncryptedFileExtensions = value.ToList();
            OnPropertyChanged(nameof(EncryptedFileExtensions));
        }
    }

    public List<string> LogTypes { get; } = ["XML", "JSON"];

    private string _selectedLogType;
    public string SelectedLogType
    {
        get => _selectedLogType;
        set
        {
            if (_selectedLogType == value) return;

            _selectedLogType = value;
            // Ensure that the value contains the expected format before splitting
            if (!string.IsNullOrEmpty(value) && value.Contains(':'))
            {
                // Extract only the log type value without the prefix "System.Windows.Controls.ComboBoxItem : "
                _config.LogType = value.Split(':')[1]!.Trim();
            }
            OnPropertyChanged(nameof(SelectedLogType));
        }
    }

    private string _processName;
    public string ProcessName
    {
        get => _processName;
        set
        {
            if (_processName == value) return;
            
            _processName = value;
            _config.ProcessName = value;
            OnPropertyChanged(nameof(ProcessName));
        }
    }

    private string _newExtensionToAdd;
    public string NewExtensionToAdd
    {
        get => _newExtensionToAdd;
        set
        {
            if (_newExtensionToAdd == value) return;
            
            _newExtensionToAdd = value;
            OnPropertyChanged(nameof(NewExtensionToAdd));
        }
    }

    public ICommand SaveCommand { get; }
    public ICommand AddExtensionCommand { get; }
    public ICommand AddFichierPrioritaireCommand { get; }

    public async Task TranslateText()
    {

        TargetLanguageText = await _translator.TranslateAsync("Target Language :");
        Extension = await _translator.TranslateAsync("Encrypted File Extensions :");
        Logi = await _translator.TranslateAsync("Processus qui empêche une sauvegarde :");
        Logtype = await _translator.TranslateAsync("Log file type :");
        AddText = await _translator.TranslateAsync("Add Extension");
        SaveText = await _translator.TranslateAsync("Sauvegarder");
        SettingText = await _translator.TranslateAsync("Settings");
        GESText = await _translator.TranslateAsync(" Extensions prioritaires");


    }

    private static string GetLanguageFromCode(string code)
    {
        return code switch
        {
            "EN" => "English",
            "FR" => "French",
            "ES" => "Spanish",
            _ => string.Empty,
        };
    }

    private void SaveConfig()
    {
        _config.SaveConfig();
    }

    private void SaveSelectedLanguage(string selectedLanguage)
    {
        switch (selectedLanguage)
        {
            case "English":
                _config.TargetLanguage = "EN";
                break;
            case "French":
                _config.TargetLanguage = "FR";
                break;
            case "Spanish":
                _config.TargetLanguage = "ES";
                break;
            // Add more languages as needed
            default:
                break;
        }
        SaveConfig(); // Save the updated config
    }

    private void AddExtension()
    {
        if (string.IsNullOrEmpty(NewExtensionToAdd))
        {
            return;
        }

        _config.EncryptedFileExtensions.Add(NewExtensionToAdd);

        EncryptedFileExtensions = new ObservableCollection<string>(_config.EncryptedFileExtensions);
        NewExtensionToAdd = string.Empty;
    }
}
