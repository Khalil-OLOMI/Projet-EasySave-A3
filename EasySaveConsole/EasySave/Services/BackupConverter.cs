using EasySave.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.Services
{
    public class BackupConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IBackup).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);

            if (jsonObject["Type"] != null)
            {
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

            throw new InvalidOperationException("Backup type information not found");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException(); // Implement if needed
        }
    }
}
