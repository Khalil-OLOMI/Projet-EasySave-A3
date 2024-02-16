using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class Config
{
    public string TargetLanguage { get; set; }
    public List<string> EncryptedFileExtensions { get; set; }
    public string ProcessName { get; set; }
    public string LogType { get; set; }

    public const string ApiKey = "3fc80bc6-26a9-9365-86c0-ddd544f23524:fx";

    private const string ConfigFilePath = "config.json";

    public Config()
    {
        TargetLanguage = null;
        EncryptedFileExtensions = new List<string>();
        ProcessName = null;
        LogType = null;
    }

    public void SaveConfig()
    {
        try
        {
            string json = JsonConvert.SerializeObject(this);
            File.WriteAllText(ConfigFilePath, json);
            Console.WriteLine("Config file saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving config file: {ex.Message}");
        }
    }

    public static Config LoadConfig()
    {
        try
        {
            if (File.Exists(ConfigFilePath))
            {
                string json = File.ReadAllText(ConfigFilePath);
                return JsonConvert.DeserializeObject<Config>(json);
            }
            else
            {
                return new Config();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading config file: {ex.Message}");
            return new Config(); // Return a new Config object in case of error
        }
    }

}
