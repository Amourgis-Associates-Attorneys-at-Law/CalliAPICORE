using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalliAPI.Models
{
    public class Task
    {
        public long id { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public long matter_id { get; set; }
    }
}
