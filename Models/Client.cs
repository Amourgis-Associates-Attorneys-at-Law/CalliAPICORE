using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CalliAPI.Models
{
    public class Client
    {
        public long? id { get; set; }

        [JsonPropertyName("name")]
        public string? client_name { get; set; }

        public string? first_name { get; set; }
        public string? middle_name { get; set; }
        public string? last_name { get; set; }
        public string? date_of_birth { get; set; }
        public string? created_at { get; set; }
        public string? updated_at { get; set; }
        public string? prefix { get; set; }
        public string? title { get; set; }
        public string? initials { get; set; }
        public string? clio_connect_email { get; set; }
        public string? primary_email_address { get; set; }

    }
}
