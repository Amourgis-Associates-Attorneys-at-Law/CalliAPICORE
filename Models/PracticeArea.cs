using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalliAPI.Models
{
    public class PracticeArea
    {
        public long id { get; set; }
        public string name { get; set; }

        public override string ToString()
        {
            return name; // This controls what shows in the UI
        }

    }
}
