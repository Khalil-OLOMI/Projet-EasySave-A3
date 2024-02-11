﻿using EasySaveConsole.Models;
using EasySaveConsole.Services;
using System;
using System.ComponentModel.Design;
using EasySaveConsole.Services;

namespace EasySaveConsole
{
    class Program
    {
        public static List<IBackup> backupList = new List<IBackup>();
        static DeepLTranslator translator;

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
            int languageChoice = Convert.ToInt32(Console.ReadLine());
            switch (languageChoice)
            {
                case 1:
                    return "EN";
                case 2:
                    return "FR";
                case 3:
                    return "ES";
                case 4:
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
            // Application title
            Console.WriteLine("EasySaveConsole");

            // Display menu options
            Console.WriteLine(await translator.TranslateAsync("Que voulez-vous faire ?"));
            Console.WriteLine(await translator.TranslateAsync("1- Ajouter un travail de sauvegarde"));
            Console.WriteLine(await translator.TranslateAsync("2- Voir la liste des travaux de sauvegarde"));
            Console.WriteLine(await translator.TranslateAsync("3- Exécuter un travail de sauvegarde"));
            Console.WriteLine(await translator.TranslateAsync("4- Consulter les logs d'un travail de sauvegarde"));
            Console.WriteLine(await translator.TranslateAsync("5- Historique des travaux de sauvegarde"));

            int menuChoice = Convert.ToInt32(Console.ReadLine());

            switch (menuChoice)
            {
                case 1:
                    Console.WriteLine(await translator.TranslateAsync("What type of backup job do you want to create?"));
                    Console.WriteLine(await translator.TranslateAsync("1- Full"));
                    Console.WriteLine(await translator.TranslateAsync("2- Differential"));

                    int backupChoice = Convert.ToInt32(Console.ReadLine());
                    switch (backupChoice)
                    {
                        case 1:
                            BackupViewModel.AddBackup("Complet");
                            break;
                        case 2:
                            BackupViewModel.AddBackup("Differential");
                            break;
                    }
                    break;
                case 2:
                    BackupViewModel.GetBackupList();
                    break;
                case 3:
               
                    break;
                case 4:
                    break;
                case 5:
                    break;
            }
            await Menu();
        }
    }
}
