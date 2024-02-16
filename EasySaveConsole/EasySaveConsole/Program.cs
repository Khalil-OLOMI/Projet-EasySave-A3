using EasySaveConsole.Models;
using EasySaveConsole.Services;
using System;
using System.ComponentModel.Design;
using EasySaveConsole.Services;

namespace EasySaveConsole
{
    class Program
    {
        public static DeepLTranslator translator;

        static async Task Main()
        {
            // Load the configuration
            var config = Config.LoadConfig();

            // Initialize DeepLTranslator with the API key
            string apiKey = Config.ApiKey;
            translator = new DeepLTranslator(apiKey);

            // If the target language is not set, prompt the user to choose
            if (string.IsNullOrEmpty(config.TargetLanguage))
            {
                config.TargetLanguage = await ChooseLanguage();
                config.SaveConfig();
            }

            await Menu();
        }

        static async Task<string> ChooseLanguage()
        {
            Console.WriteLine("\r\n    ______                 _____                        __                                           \r\n   / ____/___ ________  __/ ___/____ __   _____        / /   ____ _____  ____ ___  ______ _____ ____ \r\n  / __/ / __ `/ ___/ / / /\\__ \\/ __ `/ | / / _ \\______/ /   / __ `/ __ \\/ __ `/ / / / __ `/ __ `/ _ \\\r\n / /___/ /_/ (__  ) /_/ /___/ / /_/ /| |/ /  __/_____/ /___/ /_/ / / / / /_/ / /_/ / /_/ / /_/ /  __/\r\n/_____/\\__,_/____/\\__, //____/\\__,_/ |___/\\___/     /_____/\\__,_/_/ /_/\\__, /\\__,_/\\__,_/\\__, /\\___/ \r\n                 /____/                                               /____/            /____/       \r\n");
            Console.WriteLine("Choose your language:");
            Console.WriteLine("1. English");
            Console.WriteLine("2. French");
            Console.WriteLine("3. Spanish");
            Console.WriteLine("4. Italian");
            string languageChoice = Console.ReadLine();
            switch (languageChoice)
            {
                case "1":
                    return "EN";
                case "2":
                    return "FR";
                case "3":
                    return "ES";
                case "4":
                    return "IT";
                default:
                    Console.WriteLine("Invalid choice. Defaulting to English.");
                    return "EN";
            }
        }

        static async Task Menu()
        {
            // Title of application
            Console.WriteLine("\r\n    ______                    _____                   \r\n   / ____/____ _ _____ __  __/ ___/ ____ _ _   __ ___ \r\n  / __/  / __ `// ___// / / /\\__ \\ / __ `/| | / // _ \\\r\n / /___ / /_/ /(__  )/ /_/ /___/ // /_/ / | |/ //  __/\r\n/_____/ \\__,_//____/ \\__, //____/ \\__,_/  |___/ \\___/ \r\n                    /____/                            \r\n");

            // Display menu options
            Console.WriteLine(await translator.TranslateAsync("Que voulez-vous faire ?"));
            Console.WriteLine(await translator.TranslateAsync("1- Ajouter un travail de sauvegarde"));
            Console.WriteLine(await translator.TranslateAsync("2- Voir la liste des travaux de sauvegarde"));
            Console.WriteLine(await translator.TranslateAsync("3- Exécuter un travail de sauvegarde"));
            Console.WriteLine(await translator.TranslateAsync("4- Supprimer un travail de sauvegarde"));
            Console.WriteLine(await translator.TranslateAsync("5- Consulter l'historique de sauvegarde en temps réel"));
            Console.WriteLine(await translator.TranslateAsync("6- Consulter l'historique des travaux de sauvegarde"));
            Console.WriteLine(await translator.TranslateAsync("7- Modifier la configuration"));
            Console.WriteLine(await translator.TranslateAsync("8- Close"));

            string menuChoice = Console.ReadLine();

            switch (menuChoice)
            {
                case "1":
                    // Add backup job
                    if (BackupViewModel.backupList.Count == 5)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(await translator.TranslateAsync("Nombre maximal de sauvegarde atteint."));
                        Console.ResetColor();
                        break;
                    }
                    Console.WriteLine(await translator.TranslateAsync("What type of backup job do you want to create?"));
                    Console.WriteLine(await translator.TranslateAsync("1- Full"));
                    Console.WriteLine(await translator.TranslateAsync("2- Differential"));

                    string backupChoice = Console.ReadLine();
                    switch (backupChoice)
                    {
                        case "1":
                            await BackupViewModel.AddBackup("Complet");
                            break;
                        case "2":
                            await BackupViewModel.AddBackup("Differential");
                            break;
                    }
                    break;
                case "2":
                    // View backup list
                    await BackupViewModel.GetBackupList();
                    break;
                case "3":
                    // Execute backups
                    await BackupViewModel.ExcuteBackups();
                    break;
                case "4":
                    // Delete backup
                    await BackupViewModel.DeleteBackup();
                    break;
                case "5":
                    // Real-time backup history
                    StateViewModel.ReadStateFile();
                    break;
                case "6":
                    // View backup job history
                    LogViewModel.ReadLogFile();
                    break;
                case "7":
                    // Modify configuration
                    await ModifyConfiguration();
                    break;
                case "8":
                    // Close application
                    Environment.Exit(0);
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(await translator.TranslateAsync("Incorrect choice.")); // Choix incorrect
                    Console.ResetColor();
                    break;
            }
            await Menu();
        }

        static async Task ModifyConfiguration()
        {
            Console.WriteLine(await translator.TranslateAsync("Modify Configuration:"));
            Console.WriteLine(await translator.TranslateAsync("1. Set Target Language"));
            Console.WriteLine("2. Add Encrypted File Extension");
            Console.WriteLine("3. Set Process Name");
            Console.WriteLine("4. Set Log Type");

            string configChoice = Console.ReadLine();
            Config config = Config.LoadConfig();

            switch (configChoice)
            {
                case "1":
                    // Set Target Language
                    config.TargetLanguage = await ChooseLanguage();
                    break;
                case "2":
                    // Add Encrypted File Extension
                    Console.WriteLine("Enter Encrypted File Extension:");
                    string extension = Console.ReadLine();
                    config.EncryptedFileExtensions.Add(extension);
                    break;
                case "3":
                    // Set Process Name
                    Console.WriteLine("Enter Process Name:");
                    config.ProcessName = Console.ReadLine();
                    break;
                case "4":
                    // Set Log Type
                    Console.WriteLine("Enter Log Type (XML or JSON):");
                    string logType = Console.ReadLine();
                    config.LogType = logType;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(await translator.TranslateAsync("Invalid choice."));
                    Console.ResetColor();
                    break;
            }

            config.SaveConfig(); // Save the modified configuration
        }
    }
}
