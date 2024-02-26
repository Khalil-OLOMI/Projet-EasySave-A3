using EasySave.ViewModels;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace EasySave.Models;

public class CompletBackup : IBackup
{
    private bool isPaused;
    private bool isStopped;

    public string Name { get; set; }
    public string Source { get; set; }
    public string Cible { get; set; }
    public string Type { get; set; }
    public string Status { get; set; }

    // JB: Ici on a deux responsabilités qu'on pourrait séparer:
    // D'un coté le modèle avec les propriétés et de l'autre la classe permettant de créer une sauvegarde
    public void Copy(string source, string cible)
    {
        int nbre_file = 0;
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
                nbre_file++;
            }

            if (encryptedExtensions.Contains(fileExtension))
            {
                EncryptFile(file, targetFile);
            }

            // State tracking logic
            State state = new State()
            {
                Name = this.Name,
                Horodatage = DateTime.Now,
                Status = "ACTIVE",
                FileSource = file,
                FileTarget = targetFile,
                TotalFilesToCopy = StateViewModel.FileNbre(source),
                TotalFilesSize = StateViewModel.GetDirectorySize(source),
                NbFilesLeftToDo = StateViewModel.FileNbre(source) - nbre_file,
                Progression = StateViewModel.GetProgression(source, nbre_file),
            };

            if (config.LogType == "XML")
            {
                new StateViewModel().WriteStateXml(state);
            }
            else
            {
                new StateViewModel().WriteState(state);
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
                nbre_file++;
            }

            if (encryptedExtensions.Contains(fileExtension))
            {
                EncryptFile(file, targetFile);
            }

            // State tracking logic
            State state = new State()
            {
                Name = this.Name,
                Horodatage = DateTime.Now,
                Status = "ACTIVE",
                FileSource = file,
                FileTarget = targetFile,
                TotalFilesToCopy = StateViewModel.FileNbre(source),
                TotalFilesSize = StateViewModel.GetDirectorySize(source),
                NbFilesLeftToDo = StateViewModel.FileNbre(source) - nbre_file,
                Progression = StateViewModel.GetProgression(source, nbre_file),
            };

            if (config.LogType == "XML")
            {
                new StateViewModel().WriteStateXml(state);
            }
            else
            {
                new StateViewModel().WriteState(state);
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
}

