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

        // This can be a number, string, or null — keep as JsonElement for flexibility
        public JsonElement value { get; set; }

        public CustomFieldMetadata custom_field { get; set; }

        // NEW: Add support for picklist_option
        public PicklistOption picklist_option { get; set; }
    }

    public class CustomFieldMetadata
    {
        public long id { get; set; }
        public string etag { get; set; }
    }

    public class PicklistOption
    {
        public long? id { get; set; }
        public string option { get; set; }
    }


}
