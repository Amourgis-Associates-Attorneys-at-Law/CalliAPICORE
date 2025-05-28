using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CalliAPI.Models
{

    public class CustomFieldValue
    {
        public string id { get; set; }
        public JsonElement value { get; set; } // Can be string, number, or null
        public CustomFieldMetadata custom_field { get; set; }
    }

    public class CustomFieldMetadata
    {
        public long id { get; set; }
        public string etag { get; set; }
    }

}
