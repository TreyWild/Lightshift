using Newtonsoft.Json;


namespace Assets._Lightshift.Scripts.Data
{
    public class GameProperty
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public T Get<T>() => JsonConvert.DeserializeObject<T>(Value);
    }
}
