using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalliAPI.Interfaces
{
    public interface IReportContext
    {
        void SetReportName(string name);
        Task SetProgress(int current, int total);
    }
}
