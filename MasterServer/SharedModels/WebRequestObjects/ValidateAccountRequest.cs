using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.WebRequestObjects
{
    public class ValidateAccountRequest
    {
        public string EmailAddress { get; set; }
        public string Token { get; set; }
    }
}
