﻿using EasySave.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Windows;

namespace EasySave.Models
{
    internal class DifferentialBackup : IBackup
    {
        private bool isPaused;
        private bool isStopped;

        // Properties of the differential backup
        public string Name { get; set; }
        public string Source { get; set; }
        public string Cible { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }

        // Method to perform a differential backup
        public void Copy(string source, string cible)
        {
            Config config = Config.LoadConfig();
            List<string> PriorityExtensions = config.FichierPrioritaires;
            int nbre_file = 0;
            string[] sourceFiles = Directory.GetFiles(source, "*", SearchOption.AllDirectories);
            DecryptFilesInTarget(cible);

            foreach (string sourceFile in sourceFiles)
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

                string relativePath = sourceFile.Substring(source.Length + 1);
                string targetFile = Path.Combine(cible, relativePath);

                if (File.Exists(targetFile))
                {
                    string sourceHash = CalculateFileHash(sourceFile);
                    string targetHash = CalculateFileHash(targetFile);

                    if (sourceHash != targetHash)
                    {
                        if (PriorityExtensions.Contains(Path.GetExtension(sourceFile).TrimStart('.').ToLowerInvariant()))
                        {
                            CopyFile(sourceFile, targetFile, nbre_file);
                            nbre_file++;
                        }
                    }
                }
                else
                {
                    if (PriorityExtensions.Contains(Path.GetExtension(sourceFile).TrimStart('.').ToLowerInvariant()))
                    {
                        CopyFile(sourceFile, targetFile, nbre_file);
                        nbre_file++;
                    }
                }
            }

            // After processing priority files, copy the remaining non-priority files
            foreach (string sourceFile in sourceFiles)
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

                string relativePath = sourceFile.Substring(source.Length + 1);
                string targetFile = Path.Combine(cible, relativePath);

                if (!PriorityExtensions.Contains(Path.GetExtension(sourceFile).TrimStart('.').ToLowerInvariant()))
                {
                    // Copy the non-priority file
                    CopyFile(sourceFile, targetFile, nbre_file);
                    nbre_file++;
                }
            }

            EncryptFilesInTarget(cible);
        }

        private void CopyFile(string sourceFile, string targetFile, int nbre_file)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(targetFile));
            File.Copy(sourceFile, targetFile, true);

            State state = new()
            {
                Name = this.Name,
                Horodatage = DateTime.Now,
                Status = "ACTIVE",
                FileSource = sourceFile,
                FileTarget = targetFile,
                TotalFilesToCopy = StateViewModel.FileNbre(Source),
                TotalFilesSize = StateViewModel.GetDirectorySize(Source),
                NbFilesLeftToDo = StateViewModel.FileNbre(Source) - nbre_file,
                Progression = ((double)(StateViewModel.FileNbre(Source) - (StateViewModel.FileNbre(Source) - nbre_file)) / StateViewModel.FileNbre(Source)) * 100,
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

        // Method to calculate the hash of a file
        private string CalculateFileHash(string filePath)
        {
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
}
