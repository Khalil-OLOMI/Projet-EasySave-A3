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
        DirectoryInfo dir = new(source);

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
            }
        }

        foreach (string file in Directory.GetFiles(source))
        {

            if (isStopped)
            {
                // If the backup process is stopped, exit the loop
                return;
            }

            if (isPaused)
            {
                // If the backup process is paused, wait for it to be unpaused
                while (isPaused)
                {
                    System.Threading.Thread.Sleep(100);
                }
            }


            string targetFile = Path.Combine(cible, Path.GetFileName(file));

            File.Copy(file, targetFile, true);

            nbre_file++;

            State state = new()
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

            if (Config.LoadConfig().LogType == "XML")
            {
                new StateViewModel().WriteStateXml(state);
            }
            else
            {
                new StateViewModel().WriteState(state);
            }

            ConfigViewModel configViewModel = new(Config.LoadConfig());
            List<string> encryptedFileExtensions = configViewModel.EncryptedFileExtensions.ToList();

            var fileExtension = Path.GetExtension(file).TrimStart('.').ToLower();

            if (configViewModel.EncryptedFileExtensions.Contains(fileExtension))
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
        }

        foreach (string subdir in Directory.GetDirectories(source))
        {
            string targetSubDir = Path.Combine(cible, Path.GetFileName(subdir));
            Copy(subdir, targetSubDir);
        }

        
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

