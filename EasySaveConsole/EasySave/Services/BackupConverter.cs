using EasySave.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace EasySave.Services;
public interface IFileSystem
{
    void CopyFile(string sourceFileName, string destFileName, bool overwrite);
    void CreateDirectory(string path);
    // Autres méthodes du système de fichiers nécessaires pour CompletBackup
}
public class BackupConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return typeof(IBackup).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jsonObject = JObject.Load(reader);

        if (jsonObject["Type"] == null)
        {
            throw new InvalidOperationException("Backup type information not found");
        }

        string backupType = jsonObject["Type"].Value<string>();

        switch (backupType)
        {
            case "Complet":
                return JsonConvert.DeserializeObject<CompletBackup>(jsonObject.ToString());
            case "Differential":
                return JsonConvert.DeserializeObject<DifferentialBackup>(jsonObject.ToString());
            // Add cases for other backup types if needed
            default:
                throw new InvalidOperationException("Unknown backup type");
        }
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException(); // Implement if needed
    }
}
