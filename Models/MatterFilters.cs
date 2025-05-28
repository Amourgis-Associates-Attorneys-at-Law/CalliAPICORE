using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CalliAPI.DataAccess;

namespace CalliAPI.Models
{
    internal static class MatterFilters
    {


        /// <summary>
        /// 
        /// </summary>
        /// <param name="matters"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<Matter> FilterByStatusAsync(this IAsyncEnumerable<Matter> matters, string status)
        {
            await foreach (var matter in matters)
            {
                if (matter.status == status)
                {
                    yield return matter;
                }
            }
        }



        /// <summary>
        /// Filters matters by a custom field. For example:
        /// For a date comparison: var filtered = matters.FilterByCustomFieldAsync(CustomField.DateOf341Meeting,value => DateTime.TryParse(value, out var date) && date >= new DateTime(2025, 6, 1));
        /// </summary>
        /// <param name="matters"></param>
        /// <param name="field"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<Matter> FilterByCustomFieldAsync(
    this IAsyncEnumerable<Matter> matters,
    CustomField field,
    Func<string, bool> predicate)
{
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field), "Custom field cannot be null.");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate), "Predicate function cannot be null.");
            }

            // Get the field ID from the CustomFieldMap so we have a numeric id
            long fieldId = CustomFieldMap.GetId(field);

        await foreach (var matter in matters)
    {
                // Get the first custom field that matches the id and check if its value matches the predicate
                var match = matter.CustomFields?
            .FirstOrDefault(cf => cf.custom_field?.id == fieldId);

        if (match != null && match.value.ValueKind != JsonValueKind.Null)
        {
            string value = match.value.ToString();
            if (predicate(value))
            {
                yield return matter;
            }
}
    }
}

    //    public static async IAsyncEnumerable<Matter> FilterByCustomFieldAsync(
    //this IAsyncEnumerable<Matter> matters,
    //CustomField field,
    //Func<string, bool> predicate)
    //    {
    //        await foreach (var matter in matters)
    //        {
    //            if (matter.CustomFields.TryGetValue(field, out var value) && predicate(value))
    //            {
    //                yield return matter;
    //            }
    //        }
    //    }


        public static async IAsyncEnumerable<Matter> FilterByPracticeAreaSuffixAsync(this IAsyncEnumerable<Matter> matters, params string[] suffixes)
        {
            await foreach (var matter in matters)
            {
                if (matter.practice_area_name != null && suffixes.Any(suffix => matter.practice_area_name.EndsWith(suffix)))
                {
                    yield return matter;
                }
            }
        }

        public static async IAsyncEnumerable<Matter> FilterByStageNameAsync(this IAsyncEnumerable<Matter> matters, params string[] stageNames)
        {
            await foreach (var matter in matters)
            {
                if (matter.matter_stage_name != null && stageNames.Any(stageName => matter.matter_stage_name.Trim().ToLower().Equals(stageName.Trim().ToLower())))
                {
                    yield return matter;
                }
            }
        }


        /// <summary>
        /// Converts an IAsyncEnumerable of Matter objects to a DataTable, attempting to include all relevant properties.
        /// </summary>
        /// <param name="matters"></param>
        /// <returns></returns>
        public static async Task<DataTable> ToSmartDataTableAsync(this IAsyncEnumerable<Matter> matters)
        {
            var table = new DataTable();
            var rows = new List<Dictionary<string, object>>();
            var columns = new HashSet<string>();

            // Await foreach will consume the IAsyncEnumerable and won't break early unless we tell it to. This allows us to work on the entire collection, without worrying about potential data loss to
            // the async nature of the enumerable.
            await foreach (var matter in matters)
            {
                var row = new Dictionary<string, object>();

                // Add Matter properties
                foreach (var prop in typeof(Matter).GetProperties())
                {
                    var value = prop.GetValue(matter);
                    if (value != null & prop.Name != "client") // Skip client info for now
                    {
                        string columnName = prop.Name;
                        columns.Add(columnName);
                        row[columnName] = value;
                    }
                }

                // Add Client properties (flattened)
                if (matter.client != null)
                {
                    foreach (var clientProp in typeof(Client).GetProperties())
                    {
                        var clientValue = clientProp.GetValue(matter.client);
                        if (clientValue != null)
                        {
                            string columnName = clientProp.Name;
                            columns.Add(columnName);
                            row[columnName] = clientValue;
                        }
                    }
                }


                // Include custom fields
                if (matter.CustomFields != null)
                {
                    foreach (var field in matter.CustomFields)
                    {
                        string columnName = field.custom_field.id.ToString();
                        columns.Add(columnName);
                        row[columnName] = (field.value.ValueKind == JsonValueKind.Null) ? null : field.value.ToString();
                    }
                }

                rows.Add(row);
            }

            // Add columns to the table
            foreach (var column in columns)
            {
                table.Columns.Add(column);
            }

            // Add rows to the table
            foreach (var row in rows)
            {
                var dataRow = table.NewRow();
                foreach (var kvp in row)
                {
                    dataRow[kvp.Key] = kvp.Value;
                }
                table.Rows.Add(dataRow);
            }

            return table;
        }



        public static async Task<DataTable> ToDataTableAsync(this IAsyncEnumerable<Matter> matters)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(long));
            table.Columns.Add("Practice Area", typeof(string));
            table.Columns.Add("Stage", typeof(string));

            await foreach (var matter in matters)
            {
                table.Rows.Add(matter.id, matter.practice_area_name, matter.matter_stage_name);
            }

            return table;
        }


    }
}
