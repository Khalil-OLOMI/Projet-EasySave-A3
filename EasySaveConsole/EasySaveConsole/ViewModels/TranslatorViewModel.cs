using EasySaveConsole.Models;
using System;
using System.Text.Json;

namespace EasySaveConsole.Services
{
    class TranslatorViewModel
    {
        private static string config_path = Directory.GetCurrentDirectory() + "\\config";
        private static string config_file = Directory.GetCurrentDirectory() + "\\config\\config.json";
        public static void InitConfig()
        {
            if (Directory.Exists(config_path) == false)
            {
                Directory.CreateDirectory(config_path);
            }

            //Créer le fichier de log s'il n'existe pas déjà

            if (!File.Exists(config_file))
            {
                Config config = new Config();
                config.Language = null;
                string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(config_file, json);
            }
        }

        public static void UpdateConfig(string language)
        {
            InitConfig();
            Config config = new Config() { Language = language };
            string modifiedJson = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(config_file, modifiedJson);
        }

        public static string GetLanguage()
        {
            InitConfig();
            Config config;
            using FileStream json = File.OpenRead(config_file);
            config = JsonSerializer.Deserialize<Config>(json);
            return config.Language;
        }
    }
}
