using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;  // Importe de bibliothèque pour la sérialisation et la désérialisation JSON.
public class Config
{
    public string TargetLanguage { get; set; }
    public List<string> EncryptedFileExtensions { get; set; }
    public string ProcessName { get; set; }
    public string LogType { get; set; }

    private const string ConfigFilePath = "config.json";
    public const string ApiKey = "3fc80bc6-26a9-9365-86c0-ddd544f23524:fx";

    public Config()
    {
        TargetLanguage = null;
        EncryptedFileExtensions = new List<string>();
        ProcessName = null;
        LogType = null;
    }

    public void SaveConfig()
    {
        string json = JsonConvert.SerializeObject(this);
        File.WriteAllText(ConfigFilePath, json);
    }

    public static Config LoadConfig()
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
}


