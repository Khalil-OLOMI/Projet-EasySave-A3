using RelayCommandSET = EasySave.Helpers.RelayCommand;
using EasySave.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EasySave.Services
{
    public class ConfigViewModel : INotifyPropertyChanged
    {
        private Config _config;
        private readonly DeepLTranslator _translator;

        private string _targetLanguageText;
        public string TargetLanguageText
        {
            get { return _targetLanguageText; }
            set {_targetLanguageText = value;OnPropertyChanged();}
        }
        private string _extensionText;
        public string Extension
        {
            get { return _extensionText; }
            set { _extensionText = value; OnPropertyChanged(); }
        }

        private string _logiText;
        public string Logi
        {
            get { return _logiText; }
            set { _logiText = value; OnPropertyChanged(); }
        }

        private string _logtypeText;
        public string Logtype
        {
            get { return _logtypeText; }
            set { _logtypeText = value; OnPropertyChanged(); }
        }

        private string _addText;
        public string AddText
        {
            get { return _addText; }
            set { _addText = value; OnPropertyChanged(); }
        }

        private string _savetext;
        public string SaveText
        {
            get { return _savetext; }
            set { _savetext = value; OnPropertyChanged(); }
        }

        private bool _isEnglish;
        public bool IsEnglish
        {
            get { return _isEnglish; }
            set
            {
                if (_isEnglish != value)
                {
                    _isEnglish = value;
                    if (_isEnglish)
                    {
                        IsFrench = false;
                        IsSpanish = false;
                        SelectedLanguage = "English";
                    }
                    OnPropertyChanged();
                }
            }
        }

        private bool _isFrench;
        public bool IsFrench
        {
            get { return _isFrench; }
            set
            {
                if (_isFrench != value)
                {
                    _isFrench = value;
                    if (_isFrench)
                    {
                        IsEnglish = false;
                        IsSpanish = false;
                        SelectedLanguage = "French";
                    }
                    OnPropertyChanged();
                }
            }
        }

        private bool _isSpanish;
        public bool IsSpanish
        {
            get { return _isSpanish; }
            set
            {
                if (_isSpanish != value)
                {
                    _isSpanish = value;
                    if (_isSpanish)
                    {
                        IsEnglish = false;
                        IsFrench = false;
                        SelectedLanguage = "Spanish";
                    }
                    OnPropertyChanged();
                }
            }
        }

        private string _selectedLanguage;
        public string SelectedLanguage
        {
            get { return _selectedLanguage; }
            set
            {
                if (_selectedLanguage != value)
                {
                    _selectedLanguage = value;
                    OnPropertyChanged();
                    SaveSelectedLanguage(value);
                }
            }
        }

        public ObservableCollection<string> EncryptedFileExtensions
        {
            get => new ObservableCollection<string>(_config.EncryptedFileExtensions);
            set
            {
                _config.EncryptedFileExtensions = value.ToList();
                OnPropertyChanged();
            }
        }

        public List<string> LogTypes { get; } = new List<string> { "XML", "JSON" };

        private string _selectedLogType;
        public string SelectedLogType
        {
            get { return _selectedLogType; }
            set
            {
                if (_selectedLogType != value)
                {
                    _selectedLogType = value;
                    // Ensure that the value contains the expected format before splitting
                    if (!string.IsNullOrEmpty(value) && value.Contains(":"))
                    {
                        // Extract only the log type value without the prefix "System.Windows.Controls.ComboBoxItem : "
                        _config.LogType = value.Split(':')[1]?.Trim();
                    }
                    OnPropertyChanged();
                }
            }
        }

        private string _processName;
        public string ProcessName
        {
            get { return _processName; }
            set
            {
                if (_processName != value)
                {
                    _processName = value;
                    _config.ProcessName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _newExtensionToAdd;
        public string NewExtensionToAdd
        {
            get { return _newExtensionToAdd; }
            set
            {
                if (_newExtensionToAdd != value)
                {
                    _newExtensionToAdd = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand AddExtensionCommand { get; }


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
            TranslateText();
        }
        public async Task TranslateText()
        {
            
            TargetLanguageText = await _translator.TranslateAsync("Target Language :");
            Extension = await _translator.TranslateAsync("Encrypted File Extensions :");
            Logi = await _translator.TranslateAsync("Processus qui empêche une sauvegarde :");
            Logtype = await _translator.TranslateAsync("Log file type :");
            AddText = await _translator.TranslateAsync("Add Extension");
            SaveText = await _translator.TranslateAsync("Sauvegarder");


        }

        private string GetLanguageFromCode(string code)
        {
            switch (code)
            {
                case "EN":
                    return "English";
                case "FR":
                    return "French";
                case "ES":
                    return "Spanish";
               
                default:
                    return string.Empty;
            }
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
            if (!string.IsNullOrEmpty(NewExtensionToAdd))
            {
                _config.EncryptedFileExtensions.Add(NewExtensionToAdd);
                EncryptedFileExtensions = new ObservableCollection<string>(_config.EncryptedFileExtensions);
                NewExtensionToAdd = string.Empty;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
