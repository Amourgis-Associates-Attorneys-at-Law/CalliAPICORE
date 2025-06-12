using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CalliAPI.Models
{
    /// <summary>
    /// A representation of a matter in Clio, which includes details such as the matter's number, description, status, and associated client and practice area.
    /// </summary>
    public class Matter
    {
        /// <summary>
        /// The unique identifier for the matter in Clio.
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// The number assigned to the matter.
        /// </summary>
        public int? number { get; set; }

        /// <summary>
        /// The display number for the matter, which may include a prefix or suffix.
        /// </summary>
        public string? display_number { get; set; }

        /// <summary>
        /// The custom number for the matter, if applicable.
        /// </summary>
        public string? custom_number { get; set; }

        /// <summary>
        /// The description of the matter.
        /// </summary>
        public string? description { get; set; }

        /// <summary>
        /// The status of the matter, such as "Open", "Closed", or "Pending".
        /// </summary>
        public string? status { get; set; }
        /// <summary>
        /// The date when the matter was opened.
        /// </summary>
        public string? created_at { get; set; }

        /// <summary>
        /// The date when the matter was last updated.
        /// </summary>
        public string? updated_at { get; set; }

        /// <summary>
        /// A boolean indicating whether the matter has any tasks (regardless of completion).
        /// </summary>
        public bool has_tasks { get; set; }

        public string? matter_stage_updated_at { get; set; }

        /// <summary>
        /// The client associated with the matter.
        /// </summary>
        public Client? client { get; set; }

        /// <summary>
        /// The Practice Area associated with the matter, which may include areas like "AK7", "CL13", etc.
        /// </summary>
        public PracticeArea? practice_area { get; set; }

        /// <summary>
        /// The stage of the matter, which may include stages like "Intake", "Discovery", etc.
        /// </summary>
        public MatterStage? matter_stage { get; set; }

        //public Dictionary<CustomField, string> CustomFields { get; set; } = new Dictionary<CustomField, string>();

        /// <summary>
        /// The list of custom field values associated with the matter.
        /// </summary>
        [JsonPropertyName("custom_field_values")]
        public List<CustomFieldValue>? CustomFields { get; set; }


    }

    /// <summary>
    /// The stage of a matter in Clio, such as "Intake", "Discovery", etc.
    /// </summary>
    public class MatterStage
    {
        /// <summary>
        /// The unique identifier for the matter stage.
        /// </summary>
        public long? id { get; set; }

        /// <summary>
        /// The name of the matter stage.
        /// </summary>
        [JsonPropertyName("name")]
        public string? matter_stage_name { get; set; }

        /// <summary>
        /// The unique identifier for the practice area associated with this matter stage.
        /// </summary>
        public string? practice_area_id { get; set; }
    }
}
