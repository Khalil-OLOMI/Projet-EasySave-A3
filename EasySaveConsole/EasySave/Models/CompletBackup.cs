using EasySave.ViewModels;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace EasySave.Models
{
    public class CompletBackup : IBackup, INotifyPropertyChanged
    {
        private bool isPaused;
        private bool isStopped;
        private string LMProcessName;

        public string Name { get; set; }
        public string Source { get; set; }
        public string Cible { get; set; }
        public string Type { get; set; }

        private string _status;

        public string Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        private double _progression;
        public double Progression
        {
            get { return _progression; }
            set
            {
                if (_progression != value)
                {
                    _progression = value;
                    OnPropertyChanged(nameof(Progression));
                }
            }
        }

        private int _nbreFile = 0;
        public int NbreFile
        {
            get { return _nbreFile; }
            set
            {
                if (_nbreFile != value)
                {
                    _nbreFile = value;
                    OnPropertyChanged(nameof(NbreFile));
                }
            }
        }


        // JB: Ici on a deux responsabilités qu'on pourrait séparer:
        // D'un coté le modèle avec les propriétés et de l'autre la classe permettant de créer une sauvegarde
        public void Copy(string source, string cible)
        {
            Thread monitorThread = new Thread(MonitorBusinessSoftware);
            monitorThread.IsBackground = true;
            monitorThread.Start();

            //int nbre_file = 0;
            DirectoryInfo dir = new DirectoryInfo(source);

            if (!dir.Exists)
            {
                MessageBox.Show("Dossier source inexistant");
                return;
            }

            if (!Directory.Exists(cible))
            {
                try
                {
                    Directory.CreateDirectory(cible);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur de création de dossier cible");
                    return;
                }
            }

            Config config = Config.LoadConfig();
            List<string> priorityExtensions = config.FichierPrioritaires;
            List<string> encryptedExtensions = config.EncryptedFileExtensions;
            LMProcessName = config.ProcessName;

            foreach (string file in Directory.GetFiles(source))
            {
                if (isStopped)
                {
                    return;
                }

                if (isPaused)
                {
                    while (isPaused)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                }

                var fileExtension = Path.GetExtension(file).TrimStart('.').ToLower();
                string targetFile = Path.Combine(cible, Path.GetFileName(file));

                if (priorityExtensions.Contains(fileExtension))
                {
                    File.Copy(file, targetFile, true);
                    _nbreFile++;
                    if (encryptedExtensions.Contains(fileExtension))
                    {
                        EncryptFile(file, targetFile);
                    }
                    State state = new State()
                    {
                        Name = this.Name,
                        Horodatage = DateTime.Now,
                        Status = "ACTIVE",
                        FileSource = file,
                        FileTarget = targetFile,
                        TotalFilesToCopy = StateViewModel.FileNbre(Source),
                        TotalFilesSize = StateViewModel.GetDirectorySize(Source),
                        NbFilesLeftToDo = StateViewModel.FileNbre(Source) - _nbreFile,
                        Progression = StateViewModel.GetProgression(Source, _nbreFile),
                    };

                    Progression = state.Progression;

                    if (config.LogType == "XML")
                    {
                        new StateViewModel().WriteStateXml(state);
                    }
                    else
                    {
                        new StateViewModel().WriteState(state);
                    }
                    OnPropertyChanged(nameof(Progression));
                }
            }

            foreach (string file in Directory.GetFiles(source))
            {
                if (isStopped)
                {
                    return;
                }

                if (isPaused)
                {
                    while (isPaused)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                }

                var fileExtension = Path.GetExtension(file).TrimStart('.').ToLower();
                string targetFile = Path.Combine(cible, Path.GetFileName(file));

                if (!priorityExtensions.Contains(fileExtension) && !encryptedExtensions.Contains(fileExtension))
                {
                    File.Copy(file, targetFile, true);
                    _nbreFile++;
                    if (encryptedExtensions.Contains(fileExtension))
                    {
                        EncryptFile(file, targetFile);
                    }
                    State state = new State()
                    {
                        Name = this.Name,
                        Horodatage = DateTime.Now,
                        Status = "ACTIVE",
                        FileSource = file,
                        FileTarget = targetFile,
                        TotalFilesToCopy = StateViewModel.FileNbre(Source),
                        TotalFilesSize = StateViewModel.GetDirectorySize(Source),
                        NbFilesLeftToDo = StateViewModel.FileNbre(Source) - _nbreFile,
                        Progression = StateViewModel.GetProgression(Source, _nbreFile),
                    };

                    Progression = state.Progression;

                    if (config.LogType == "XML")
                    {
                        new StateViewModel().WriteStateXml(state);
                    }
                    else
                    {
                        new StateViewModel().WriteState(state);
                    }
                    OnPropertyChanged(nameof(Progression));
                }
            }

            foreach (string subdir in Directory.GetDirectories(source))
            {
                string targetSubDir = Path.Combine(cible, Path.GetFileName(subdir));
                Copy(subdir, targetSubDir);
            }
        }

        private void EncryptFile(string sourceFile, string targetFile)
        {
            // Call CryptoSoft to encrypt the file
            string encryptedFile = targetFile + ".encrypted";

            ProcessStartInfo processStartInfo = new("CryptoSoft.exe", $"\"{targetFile}\" \"{encryptedFile}\"")
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };
            Process.Start(processStartInfo)?.WaitForExit();

            File.Delete(targetFile);
            File.Move(encryptedFile, targetFile);
        }

        public void Play()
        {
            isPaused = false;
            isStopped = false;
        }

        public void Pause()
        {
            isPaused = true;
        }

        public void Stop()
        {
            isStopped = true;
        }

        public bool IsPaused()
        {
            return isPaused;
        }

        public void Resume()
        {
            isPaused = false;
        }

        private void MonitorBusinessSoftware()
        {
            while (true)
            {

                bool isCurrentlyRunning = IsBusinessSoftwareRunning();

                if (isCurrentlyRunning)
                {
                    isPaused = true;
                    OnPropertyChanged(nameof(IsPaused));
                    MessageBox.Show($"Le processus {LMProcessName} est en cours d'exécution. Veuillez fermer toutes ses instances pour reprendre la sauvegarde.", "Processus en cours d'exécution", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                Thread.Sleep(100);
            }
        }

        private bool IsBusinessSoftwareRunning()
        {
            Process[] processes = Process.GetProcessesByName(LMProcessName);
            return processes.Length > 0;
        }



        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}