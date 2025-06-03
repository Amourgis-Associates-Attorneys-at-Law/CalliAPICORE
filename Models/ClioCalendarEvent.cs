using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalliAPI.Models
{
    public class ClioCalendarEvent
    {
        public string id { get; set; }
        public string summary { get; set; }
        public string description { get; set; }
        public string location { get; set; }
        public string start_at { get; set; }
        public string end_at { get; set; }
        public bool all_day { get; set; }
        public ClioCalendarEventMatter matter { get; set; }

    }


    public class ClioCalendarEventMatter
    {
        public long id { get; set; }
        public string description { get; set; }
        public string display_number { get; set; }
        public string status { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
    }
}
