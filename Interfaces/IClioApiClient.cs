using CalliAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CalliAPI.Interfaces
{
    internal interface IClioApiClient
    {

        Task<HttpResponseMessage> VerifyAPI(string accessToken);
        Task<string> GetAccessTokenAsync(string authorizationCode);
        Task<List<Matter>> GetAllActive713MattersAsync(string accessToken);
        Task<List<Matter>> GetMattersNotCurrentlyBeingWorked(string accessToken);

    }
}
