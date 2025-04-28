using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalliAPI.Interfaces
{
    internal interface IAuthService
    {

        string GetAuthorizationUrl();
        string ValidateAuthorizationCode(string userInput);
        Task GetAccessTokenAsync(string authorizationCode);
        string AccessToken { get; }

    }
}
