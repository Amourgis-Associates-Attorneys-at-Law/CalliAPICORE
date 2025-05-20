using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalliAPI.Interfaces;
using CalliAPI.Models;
using AmourgisCOREServices;

namespace CalliAPI.BusinessLogic
{
    internal class ClioService : IClioService
    {
        private static readonly AMO_Logger _logger = new AMO_Logger(typeof(ClioService).FullName);

        private readonly IClioApiClient _clioApiClient;

        public ClioService(IClioApiClient clioApiClient)
        {
            _clioApiClient = clioApiClient;
        }

        public async Task<string> VerifyAPI()
        {
            return await _clioApiClient.VerifyAPI();
        }

        public async Task<List<Matter>> GetMattersNotCurrentlyBeingWorked()
        {
            return await _clioApiClient.GetMattersNotCurrentlyBeingWorked();
        }


        public async Task<DataTable> GetMattersNotCurrentlyBeingWorkedAsDataTable()
        {
            var matters = await _clioApiClient.GetMattersNotCurrentlyBeingWorked();
            return ConvertToDataTable(matters);
        }

        public async Task<DataTable> GetAllMattersAsDataTable()
        {
            var matters = await _clioApiClient.GetAllActive713MattersAsync();
            return ConvertToDataTable(matters);
        }


        private DataTable ConvertToDataTable(List<Matter> matters)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id", typeof(long));
            table.Columns.Add("Practice Area", typeof(string));
            table.Columns.Add("Stage", typeof(string));

            foreach (var matter in matters)
            {
                table.Rows.Add(matter.id, matter.practice_area_name, matter.matter_stage_name);
            }

            return table;
        }



    }
}
