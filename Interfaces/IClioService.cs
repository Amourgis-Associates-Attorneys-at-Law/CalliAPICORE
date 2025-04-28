using CalliAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalliAPI.Interfaces
{
    internal interface IClioService
    {

        Task<string> VerifyAPI();
        Task<List<Matter>> GetMattersNotCurrentlyBeingWorked();

        Task<DataTable> GetMattersNotCurrentlyBeingWorkedAsDataTable();
        Task<DataTable> GetAllMattersAsDataTable();
    }
}
