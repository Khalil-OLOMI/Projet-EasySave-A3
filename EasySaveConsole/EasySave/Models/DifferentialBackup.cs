using EasySave.ViewModels;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Windows;

namespace EasySave.Models;

internal class DifferentialBackup : IBackup
{
    private bool isPaused;
    private bool isStopped;
    // Propriétés de la sauvegarde différentielle
    public string Name { get; set; } // Nom de la sauvegarde
    public string Source { get; set; } // Répertoire source
    public string Cible { get; set; } // Répertoire cible
    public string Type { get; set; } //Type of backup
    public string Status { get; set; } // Status of backup

    // Méthode pour effectuer une sauvegarde différentielle
    public void Copy(string source, string cible)
    {
        int nbre_file = 0;
        // Obtenir la liste des fichiers dans le répertoire source
        string[] sourceFiles = Directory.GetFiles(source, "*", SearchOption.AllDirectories);
        DecryptFilesInTarget(cible);

        // Parcourir chaque fichier dans le répertoire source
        foreach (string sourceFile in sourceFiles)
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
            // Obtenir le chemin relatif du fichier
            string relativePath = sourceFile.Substring(source.Length + 1);

            // Construire le chemin du fichier cible
            string targetFile = Path.Combine(cible, relativePath);

            // Vérifier si le fichier source existe dans le répertoire cible
            if (File.Exists(targetFile))
            {
                // Calculer les hashs pour les fichiers source et cible
                string sourceHash = CalculateFileHash(sourceFile);
                string targetHash = CalculateFileHash(targetFile);


                // Si les hashs sont différents, copier le fichier
                if (sourceHash != targetHash)
                {
                    CopyFile(source, sourceFile, targetFile, nbre_file);

                    nbre_file++;
                }
            }
            else
            {
                CopyFile(source, sourceFile, targetFile, nbre_file);

                nbre_file++;
            } 
        }
        EncryptFilesInTarget(cible);
    }

    private void CopyFile(string source, string sourceFile, string targetFile, int nbre_file)
    {
        // Créer le répertoire s'il n'existe pas
        Directory.CreateDirectory(Path.GetDirectoryName(targetFile));

        // Copier le fichier
        File.Copy(sourceFile, targetFile, true);

        State state = new()
        {
            Name = this.Name,
            Horodatage = DateTime.Now,
            Status = "ACTIVE",
            FileSource = sourceFile,
            FileTarget = targetFile,
            TotalFilesToCopy = StateViewModel.FileNbre(source),
            TotalFilesSize = StateViewModel.GetDirectorySize(source),
            NbFilesLeftToDo = StateViewModel.FileNbre(source) - nbre_file,
            Progression = ((double)(StateViewModel.FileNbre(source) - (StateViewModel.FileNbre(source) - nbre_file)) / StateViewModel.FileNbre(source)) * 100,
        };

        WriteState(state);
    }

    private static void WriteState(State state)
    {
        switch (Config.LoadConfig().LogType)
        {
            case "XML":
                new StateViewModel().WriteStateXml(state);
                break;
            default:
                new StateViewModel().WriteState(state);
                break;
        }
    }

    // Méthode pour calculer le hash d'un fichier
    private string CalculateFileHash(string filePath)
    {
        // JB: Pourquoi avoir utilisé un hash ici? on peut vérifier la date de modification du fichier?
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(filePath);

        byte[] hash = md5.ComputeHash(stream);

        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    private void DecryptFilesInTarget(string directory)
    {
        string[] encryptedFiles = Directory.GetFiles(directory, "*encrypted.*", SearchOption.AllDirectories);

        foreach (string encryptedFile in encryptedFiles)
        {
            try
            {
                // Decrypt the encrypted file
                string decryptedFile = Path.Combine(Path.GetDirectoryName(encryptedFile), Path.GetFileNameWithoutExtension(encryptedFile).Replace("encrypted.", ""));
                ProcessStartInfo psi = new ProcessStartInfo("CryptoSoft.exe", $"\"{encryptedFile}\"");
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                Process.Start(psi)?.WaitForExit();

                // Delete the encrypted file
                File.Delete(encryptedFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"La suppression du fichier chiffré a échoué : {encryptedFile}\nErreur : {ex.Message}", "Erreur de déchiffrement", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void EncryptFilesInTarget(string directory)
    {
        try
        {
            ConfigViewModel configViewModel = new ConfigViewModel(Config.LoadConfig());
            List<string> encryptedFileExtensions = configViewModel.EncryptedFileExtensions.Select(ext => ext.ToLowerInvariant()).ToList();

            string[] targetFiles = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);

            foreach (string targetFile in targetFiles)
            {
                try
                {
                    string fileExtension = Path.GetExtension(targetFile).TrimStart('.').ToLowerInvariant();

                    if (!targetFile.StartsWith("encrypted.") && encryptedFileExtensions.Contains(fileExtension))
                    {
                        string fileName = Path.GetFileNameWithoutExtension(targetFile);
                        string encryptedFileName = "encrypted." + fileName + Path.GetExtension(targetFile);
                        string encryptedFilePath = Path.Combine(Path.GetDirectoryName(targetFile), encryptedFileName);

                        ProcessStartInfo psi = new ProcessStartInfo("CryptoSoft.exe", $"\"{targetFile}\" \"{encryptedFilePath}\"");
                        psi.CreateNoWindow = true;
                        psi.UseShellExecute = false;
                        Process.Start(psi)?.WaitForExit();

                        File.Delete(targetFile);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Le chiffrement du fichier a échoué : {targetFile}\nErreur : {ex.Message}", "Erreur de chiffrement", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Une erreur s'est produite lors du chiffrement des fichiers : {ex.Message}", "Erreur de chiffrement", MessageBoxButton.OK, MessageBoxImage.Error);
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
