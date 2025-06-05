using AmourgisCOREServices;
using CalliAPI.BusinessLogic;
using CalliAPI.DataAccess;
using CalliAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using Task = System.Threading.Tasks;


namespace CalliAPI.Models
{
    internal static class MatterFilters
    {
        static readonly AMO_Logger _logger = new("CalliAPI");



        /// <summary>
        /// Async logging function so we can log counts without disrupting streaming. Writes a LOT of text (one line per item in source plus one line to total)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="logger"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<T> LogEachAsync<T>(this IAsyncEnumerable<T> source, AMO_Logger logger, string label)
        {
            int count = 0;
            await foreach (var item in source)
            {
                count++;
                logger.Info($"{label} - Item #{count}: {item}");
                yield return item;
            }
            logger.Info($"{label} - Total items: {count}");
        }

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
        /// For a date comparison: var filtered = matters.FilterByCustomFieldAsync(CustomField.DateOf341Meeting,value => DateTime.TryParse(value, out var date) and date >= new DateTime(2025, 6, 1));
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
            string value = match.value.ToString() ?? "null";
            if (predicate(value))
            {
                yield return matter;
            }
}
    }
}




        public static async IAsyncEnumerable<Matter> FilterByPracticeAreaSuffixAsync(this IAsyncEnumerable<Matter> matters, params string[] suffixes)
        {
            await foreach (var matter in matters)
            {
                if (matter.practice_area != null 
                    && matter.practice_area.name != null 
                    && suffixes.Any(suffix => matter.practice_area.name.EndsWith(suffix)))
                {
                    yield return matter;
                }
            }
        }

        public static async IAsyncEnumerable<Matter> FilterByStageNameAsync(this IAsyncEnumerable<Matter> matters, params string[] stageNames)
        {
            await foreach (var matter in matters)
            {
                if (matter.matter_stage != null 
                    && matter.matter_stage.name != null 
                    && stageNames.Any(stageName => matter.matter_stage.name.Trim().ToLower().Equals(stageName.Trim().ToLower())))
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
        public static async Task<DataTable> ToSmartDataTableAsync(this IAsyncEnumerable<Matter> matters,
            List<CustomField>? includedCustomFields = null)
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
                    if (value != null && prop.Name != "client" && prop.Name != "matter_stage") // Skip client info for now
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

                // Add PracticeArea properties (flattened)
                if (matter.practice_area != null)
                {
                    foreach (var practiceAreaProp in typeof(PracticeArea).GetProperties())
                    {
                        var practiceAreaValue = practiceAreaProp.GetValue(matter.practice_area);
                        if (practiceAreaValue != null)
                        {
                            string columnName = practiceAreaProp.Name;
                            columns.Add(columnName);
                            row[columnName] = practiceAreaValue;
                        }
                    }
                }

                // Add MatterStage properties (flattened)
                if (matter.matter_stage != null)
                {
                    foreach (var matterStageProp in typeof(MatterStage).GetProperties())
                    {
                        var matterStageValue = matterStageProp.GetValue(matter.matter_stage);
                        if (matterStageValue != null)
                        {
                            string columnName = matterStageProp.Name;
                            columns.Add(columnName);
                            row[columnName] = matterStageValue;
                        }
                    }
                }

                // Include custom fields
                // Include custom fields
                if (matter.CustomFields != null)
                {
                    foreach (var field in matter.CustomFields)
                    {

                        if (includedCustomFields != null &&
                         (!CustomFieldMap.TryGetField(field.custom_field.id, out var enumField) ||
                         !includedCustomFields.Contains(enumField)))
                        {
                            continue; // Skip fields not in the list
                        }


                        string columnName = field.field_name ?? $"Field_{field.custom_field.id}";
                        columns.Add(columnName);

                        // Prefer picklist label if available
                        if (field.picklist_option?.option != null)
                        {
                            row[columnName] = field.picklist_option.option;
                        }
                        else if (field.value.ValueKind != JsonValueKind.Null)
                        {
                            row[columnName] = field.value.ToString();
                        }
                        else
                        {
                            row[columnName] = "null";
                        }
                    }

                }


                rows.Add(row);
            }

            // Add columns to the table
            foreach (var column in columns.OrderBy(c => c, StringComparer.OrdinalIgnoreCase))
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
                table.Rows.Add(matter.id, matter.practice_area?.name ?? "", matter.matter_stage?.name ?? "");
            }

            return table;
        }


    }
}
