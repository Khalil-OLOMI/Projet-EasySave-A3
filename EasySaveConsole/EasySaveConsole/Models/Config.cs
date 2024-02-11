using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;  // Importe de bibliothèque pour la sérialisation et la désérialisation JSON.

public class Config
{
    public string TargetLanguage { get; set; }  // Déclare une propriété publique de type chaîne appelée TargetLanguage.

    public const string ApiKey = "3fc80bc6-26a9-9365-86c0-ddd544f23524:fx";  // Clé Api Deepl.

    private const string ConfigFilePath = "Language-config.json";

    public Config()  // Déclare un constructeur par défaut de la classe Config.
    {
        TargetLanguage = null; // Initialisez la langue cible à null par défaut.
    }

    public void SaveConfig()
    {
        string json = JsonConvert.SerializeObject(this);
        File.WriteAllText(ConfigFilePath, json);  // Écrit le contenu JSON sérialisé dans un fichier spécifié par ConfigFilePath.
    }

    public static Config LoadConfig()
    {
        if (File.Exists(ConfigFilePath))  // Vérifie si le fichier de configuration existe.
        {
            string json = File.ReadAllText(ConfigFilePath);  // Lit le contenu JSON à partir du fichier de configuration.
            return JsonConvert.DeserializeObject<Config>(json);  // Désérialise le contenu JSON en un objet Config à l'aide de la bibliothèque Newtonsoft.Json.
        }
        else
        {
            // Créez un nouvel objet Config si le fichier n'existe pas.
            return new Config();
        }
    }
}


