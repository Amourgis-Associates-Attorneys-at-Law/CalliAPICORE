using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// 
        /// </summary>
        /// <param name="matters">An enumerable list of Matter objects.</param>
        /// <param name="status">A string representing status ("open", "closed", or "pending").</param>
        /// <returns></returns>
        public static IEnumerable<Matter> FilterByStatus(this IEnumerable<Matter> matters, string status)
        {
            return matters.Where(m => m.status == status);
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
            await foreach (var matter in matters)
            {
                if (matter.CustomFields.TryGetValue(field, out var value) && predicate(value))
                {
                    yield return matter;
                }
            }
        }


        /// <summary>
        /// Filters matters by a custom field. For example:
        /// For a date comparison: var filtered = matters.FilterByCustomField(CustomField.DateOf341Meeting,value => DateTime.TryParse(value, out var date) && date >= new DateTime(2025, 6, 1));
        /// </summary>
        /// <param name="matters">An enumerable list of Matter objects.</param>
        /// <param name="custom_field_id"></param>
        /// <param name="predicate">Logic to return</param>
        /// <returns></returns>
        public static IEnumerable<Matter> FilterByCustomField(
            this IEnumerable<Matter> matters,
            CustomField custom_field_id,
            Func<string, bool> predicate)
        {
            return matters.Where(m =>
                m.CustomFields != null &&
                m.CustomFields.TryGetValue(custom_field_id, out var fieldValue) &&
                predicate(fieldValue)
            );
        }


        public static IEnumerable<Matter> FilterByPracticeAreaSuffix(this IEnumerable<Matter> matters, params string[] suffixes)
        {
            return matters.Where(m => suffixes.Any(suffix => m.practice_area_name?.EndsWith(suffix) == true));
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

                foreach (var prop in typeof(Matter).GetProperties())
                {
                    var value = prop.GetValue(matter);
                    if (value != null)
                    {
                        string columnName = prop.Name;
                        columns.Add(columnName);
                        row[columnName] = value;
                    }
                }

                // Include custom fields
                if (matter.CustomFields != null)
                {
                    foreach (var kvp in matter.CustomFields)
                    {
                        if (!string.IsNullOrWhiteSpace(kvp.Value))
                        {
                            string columnName = kvp.Key.ToString();
                            columns.Add(columnName);
                            row[columnName] = kvp.Value;
                        }
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
