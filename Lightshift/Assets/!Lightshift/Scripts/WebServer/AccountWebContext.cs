using Proyecto26;
using SharedModels.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AccountWebContext : WebContext
{
    public AccountWebContext(string controller) : base(controller) 
    {
        
    }

    public void GetAccountById<T>(string id, Action<T> callback)
    {
        RestClient.Get<T>($"{DB.serverPath}{controller}/get/{id}").Then(response => {
            callback(response);
        });
    }

    public void GetAccountByEmail<T>(string email, Action<T> callback)
    {
        RestClient.Get<T>($"{DB.serverPath}{controller}/getbyemail/{email}").Then(response => {
            callback(response);
        });
    }

    public void CheckEmailAvailability(string email, Action<bool> callback)
    {
        RestClient.Get<bool>($"{DB.serverPath}{controller}/checkemailavailability/{email}").Then(response => {
            callback(response);
        });
    }

    public void CheckUsernameAvailability(string username, Action<bool> callback)
    {
        RestClient.Get<bool>($"{DB.serverPath}{controller}/checkusernameavailability/{username}").Then(response => {
            callback(response);
        });
    }
}