using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets._Lightshift.Scripts.Data
{
    public class GameProperty
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public T Get<T>() => JsonConvert.DeserializeObject<T>(Value);
    }
}
