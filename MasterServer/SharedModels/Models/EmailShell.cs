using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Models
{
    public class EmailShell
    {
        public string emailAddress { get; set; }
        public string message { get; set; }
        public string subject { get; set; }
        public string username { get; set; }
    }
}
