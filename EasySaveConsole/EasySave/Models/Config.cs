using System.IO;
using Newtonsoft.Json;  // Importe de bibliothèque pour la sérialisation et la désérialisation JSON.
public class Config
{
    private const string ConfigFilePath = "config.json";
    public const string ApiKey = "2f5fbc6f-3655-48de-898e-e443eae45379:fx";

    public string TargetLanguage { get; set; } = null;
    public List<string> EncryptedFileExtensions { get; set; } = new List<string>();
    public string ProcessName { get; set; } = null;
    public string LogType { get; set; } = null;

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


