using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CalliAPI.Models
{
    public class Matter
    {
        public long id { get; set; }
        public int number { get; set; }
        public string display_number { get; set; }
        public string custom_number { get; set; }
        public string description { get; set; }
        public string status { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public bool has_tasks { get; set; }
        public string matter_stage_name { get; set; }
        
        public string practice_area_name { get; set; }

        public Client client { get; set; }

        //public Dictionary<CustomField, string> CustomFields { get; set; } = new Dictionary<CustomField, string>();

        [JsonPropertyName("custom_field_values")]
        public List<CustomFieldValue> CustomFields { get; set; }

    }
}
