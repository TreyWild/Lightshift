using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuthMessage : MessageBase
{
    public string authKey;
    public string connectUserId;

    public override string ToString() 
    {
        return $"UserID: {connectUserId}, key: {authKey}";
    }
}
