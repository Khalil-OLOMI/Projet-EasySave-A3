using Newtonsoft.Json;
using System.Xml.Serialization;

namespace EasySave.Models;

// JB: public?
public class State
{
    [JsonProperty("Horodatage")]
    [JsonConverter(typeof(CustomDateTimeConverter))]
    public DateTime Horodatage { get; set; }
    [XmlElement("Horodatage", Namespace = "clr-namespace:EasySaveConsole.Models;assembly=EasySaveConsole")]
    public string XamlHorodatage
    {
        get => Horodatage.ToString("dd/MM/yyyy HH:mm:ss");
        set => Horodatage = DateTime.Parse(value);
    }
    public string Name { get; set; }
    public string Status { get; set; }
    public string FileSource { get; set; }
    public string FileTarget { get; set; }
    public int TotalFilesToCopy { get; set; }
    public long TotalFilesSize { get; set; }
    public int NbFilesLeftToDo { get; set; }
    public double Progression { get; set; }
}

