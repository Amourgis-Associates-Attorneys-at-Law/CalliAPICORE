using CalliAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalliAPI.Interfaces
{
    internal interface IClioApiClient
    {

        Task<string> VerifyAPI();
        Task<string> GetAccessTokenAsync(string authorizationCode);
        Task<List<Matter>> GetAllActive713MattersAsync();
        Task<List<Matter>> GetMattersNotCurrentlyBeingWorked();

    }
}
