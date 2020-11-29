using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.WebRequestObjects
{
    public enum AuthenticationResponseType
    {
        InvalidCredentials,
        ValidationRequired,
        ValidationFail,
        LoginFail,
        EmailTaken,
        UsernameTaken,
        RecoverSuccess,
        RecoverFail,
        Banned,
        AuthenticationFail,
        AuthenticationSuccess,
        RegisterSuccess,
        RegisterFail,
        LoginSuccess,
        RequestNewConfirmationCode
    }
    public class AuthenticationResponse
    {
        public AuthenticationResponseType ResponseType { get; set; }
        public string Message { get; set; }
    }
}
