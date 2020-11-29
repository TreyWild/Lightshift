
using MasterServer.Services;
using Microsoft.AspNetCore.Mvc;
using SharedModels;
using SharedModels.Models.User;
using SharedModels.WebRequestObjects;
using System;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MasterServer.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class AccountController : ControllerBase
    {

        [HttpPost("login")]
        public AuthenticationResponse Login(LoginRequest login)
        {
            // INVALID JSON
            if (login == null)
                return new AuthenticationResponse 
                {
                    ResponseType = AuthenticationResponseType.LoginFail,
                    Message = "Invalid Json"
                };

            // LOAD ACCOUNT
            var account = DB.Accounts.GetByEmail(login.Email);

            if (account == null)
                return new AuthenticationResponse
                {
                    ResponseType = AuthenticationResponseType.LoginFail,
                    Message = "Invalid Credentials"
                };

            var userCredentials = DB.Credentials.GetUserCredentials(account.Id);

            if (PasswordHasher.ValidatePassword(login.Password, userCredentials.HashedPassword))
            {
                // LOGIN SUCCESS

                // CREATE NEW USER SESSION
                var authKey = AuthenticationService.CreateSession(account.Id, Request);
                account.Profile.TotalSessions++;
                account.Profile.LastActivity = DateTime.Now;

                //SAVE ACCOUNT
                DB.Accounts.SaveDocument(account);

                // RETURN USER MODEL
                return new AuthenticationResponse
                {
                    ResponseType = AuthenticationResponseType.LoginSuccess,
                    Message = authKey
                };
            }

            // LOGIN FAILED
            else return new AuthenticationResponse
            {
                ResponseType = AuthenticationResponseType.LoginFail,
                Message = "Invalid Credentials"
            };
        }

        //[HttpPost("validateAccount")]
        //public ActionResult<string> ValidateAccount(ValidateAccountRequest validationRequest)
        //{
        //    // INVALID JSON
        //    if (validationRequest == null)
        //        return null;

        //    // LOAD ACCOUNT
        //    var account = DB.Accounts.GetByEmail(validationRequest.EmailAddress);

        //    if (account == null || account.AccountConfirmed)
        //        return null;

        //    var userCredentials = DB.Credentials.GetUserCredentials(account.Id);

        //    if (validationRequest.Token != userCredentials.ValidationToken) 
        //    {
        //        userCredentials.ValidationToken = $"{new Random().Next(159248, 985214)}";
        //        EmailService.Sen
        //    }

        //    // LOGIN FAILED
        //    else return null;
        //}

        [HttpPost("register")]
        public AuthenticationResponse Register(RegisterRequest register)
        {
            // INVALID JSON
            if (register == null)
                return new AuthenticationResponse
                {
                    ResponseType = AuthenticationResponseType.RegisterFail,
                    Message = "Invalid Json"
                };

            // LOAD ACCOUNT
            var emailAvailable = DB.Accounts.EmailAvailable(register.Email);

            // EMAIL ALREADY IN USE OR INVALID
            if (!emailAvailable || !(register.Email.Contains('@') && register.Email.Contains('.')))
                return new AuthenticationResponse
                {
                    ResponseType = AuthenticationResponseType.EmailTaken,
                    Message = "Email not valid."
                };

            var usernameAvailable = DB.Accounts.UsernameAvailable(register.Username);
            if (!usernameAvailable || register.Username.Length > 20 || register.Username.Length < 2)
                return new AuthenticationResponse
                {
                    ResponseType = AuthenticationResponseType.UsernameTaken,
                    Message = "Username not available."
                };

            string userId = Guid.NewGuid().ToString();

            var account = new Account
            {
                Id = userId,
                EmailAddress = register.Email.ToLowerInvariant(),
                CaseSensitiveUsername = register.Username.ToUpper(),

                Profile = new Profile
                {
                    CreationDate = DateTime.Now,
                    LastActivity = DateTime.Now,
                    Level = 1,
                    TotalSessions = 1,
                    Username = register.Username,
                    XP = 0,
                }
            };

            //SAVE ACCOUNT
            DB.Accounts.SaveDocument(account);

            //SAVE PASSWORD
            DB.Credentials.CreateUserCredentials(new SharedModels.Models.UserCredentials
            {
                HashedPassword = PasswordHasher.HashPassword(register.Password),
                UserId = userId,
                ValidationToken = $"{new Random().Next(159248, 985214)}"
            });

            //CREATE SESSION
            var session = AuthenticationService.CreateSession(userId, Request);
            return new AuthenticationResponse
            {
                ResponseType = AuthenticationResponseType.RegisterSuccess,
                Message = session
            };
        }

        [HttpPost("Save")]
        public ActionResult<Account> Save(Account account)
        {
            if (!AuthenticationService.ValidateGameServerFromRequest(Request))
                return null;

            return DB.Accounts.SaveDocument(account);
        }

        [HttpPost("authenticate")]
        public ActionResult<JsonString> AuthenticateUser(JsonString json)
        {
            if (!AuthenticationService.ValidateGameServerFromRequest(Request))
                return null;

            var account =  AuthenticationService.GetAccountFromSessionKey(json.Value);

            if (account != null)
                return new JsonString { Value = account.Id };
            else return null;
        }

        [HttpPost("get")]
        public ActionResult<Account> GetAccount(JsonString json)
        {
            if (!AuthenticationService.ValidateGameServerFromRequest(Request))
                return null;

            return DB.Accounts.GetById(json.Value);
        }
    }
}
