using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CalliAPI.Models
{
    /// <summary>
    /// Holds the value of a custom field for a matter in Clio.
    /// </summary>
    public class CustomFieldValue
    {
        public string id { get; set; }

        // This can be a number, string, or null — keep as JsonElement for flexibility
        public JsonElement value { get; set; }
        
        public string field_name { get; set; }

        public CustomFieldMetadata custom_field { get; set; }

        // NEW: Add support for picklist_option
        public PicklistOption picklist_option { get; set; }
    }
    /// <summary>
    /// Holds the metadata for a custom field in Clio.
    /// </summary>
    public class CustomFieldMetadata
    {
        public long id { get; set; }
        public string etag { get; set; }
    }

    /// <summary>
    /// Holds the metadata for a picklist option in Clio.
    /// </summary>
    public class PicklistOption
    {
        public long? id { get; set; }
        public string option { get; set; }
    }

    /// <summary>
    /// Holds the definition of a custom field in Clio.
    /// </summary>
    public class CustomFieldDefinition
    {
        public long id { get; set; }
        public string name { get; set; }
        public string parent_type { get; set; }
        public string field_type { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class CustomFieldResponse
    {
        public List<CustomFieldDefinition> data { get; set; }
    }


}
