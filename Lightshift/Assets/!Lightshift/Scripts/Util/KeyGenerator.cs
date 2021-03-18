using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets._Lightshift.Scripts.Util
{
    public class KeyGenerator
    {
        private static readonly string _allowedCharacters = "ASDFZXCVQWERTPLMO";

        public static string Generate(int numberOfCharacters)
        {
            const int from = 0;
            int to = _allowedCharacters.Length;
            Random r = new Random();

            StringBuilder qs = new StringBuilder();
            for (int i = 0; i < numberOfCharacters; i++)
            {
                qs.Append(_allowedCharacters.Substring(r.Next(from, to), 1));
            }
            return qs.ToString().ToUpper();
        }
    }
}
