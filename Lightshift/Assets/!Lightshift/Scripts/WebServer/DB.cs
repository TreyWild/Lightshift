using Proyecto26;
using SharedModels.Models.User;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class DB
{
    public const string serverPath = "https://localhost.:44333/";

    public static AccountWebContext Accounts = new AccountWebContext("account");
}