using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AlertMessage : NetworkMessage
{
    public string Message;
    public bool IsPopup = true;
}

