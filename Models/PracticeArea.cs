using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CalliAPI.Models
{
    public class PracticeArea
    {
        public long id { get; set; }

        [JsonPropertyName("name")]
        public string practice_area_name { get; set; }

        public override string ToString()
        {
            return practice_area_name; // This controls what shows in the UI
        }

    }
}
