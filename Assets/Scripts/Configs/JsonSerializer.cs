using Newtonsoft.Json;
using WanzyeeStudio.Json;

namespace Configs
{
    public interface IJsonSerializer
    {
        string Serialize(object source);
        T Deserialize<T>(string jsonText);
    }

    public class JsonSerializer : IJsonSerializer
    {
        public string Serialize(object source)
        {
            return JsonConvert.SerializeObject(source, Formatting.Indented, JsonNetUtility.defaultSettings);
        }

        public T Deserialize<T>(string jsonText)
        {
            return JsonConvert.DeserializeObject<T>(jsonText, JsonNetUtility.defaultSettings);
        }
    }
}