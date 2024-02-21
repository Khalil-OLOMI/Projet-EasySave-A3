using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Xml.Serialization;

namespace EasySave.Models;

// JB: pour les modèles c'est une bonne pratique d'ajouter le suffixe Model
// On peut aussi ajouter des description pour chaque propriété
public class Log
{
    [JsonProperty("Horodatage")]
    [JsonConverter(typeof(CustomDateTimeConverter))]
    public DateTime Horodatage { get; set; }

    [XmlElement("Horodatage", Namespace = "clr-namespace:EasySaveConsole.Models;assembly=EasySaveConsole")]
    public string XmlHorodatage
    {
        get => Horodatage.ToString("dd/MM/yyyy HH:mm:ss");
        set => Horodatage = DateTime.Parse(value);
    }

    /// <summary>
    /// Ajouter des descriptions
    /// </summary>
    public string Name { get; set; }
    public string FileSource { get; set; }
    public string FileTarget { get; set; }
    public long FileSize { get; set; }
    public long FileTransferTime { get; set; }
}

public class CustomDateTimeConverter : IsoDateTimeConverter
{
    public CustomDateTimeConverter()
    {
        DateTimeFormat = "dd/MM/yyyy HH:mm:ss";
    }
}
