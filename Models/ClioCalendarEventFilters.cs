using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalliAPI.Models
{
    public static class ClioCalendarEventFilters
    {
        public static async Task<DataTable> ToSmartDataTableAsync(this IAsyncEnumerable<ClioCalendarEvent> events)
        {
            var table = new DataTable();
            var rows = new List<Dictionary<string, object>>();
            var columns = new HashSet<string>();

            await foreach (var calendarEvent in events)
            {
                var row = new Dictionary<string, object>();

                foreach (var prop in typeof(ClioCalendarEvent).GetProperties())
                {
                    var value = prop.GetValue(calendarEvent);
                    if (value != null)
                    {
                        string columnName = prop.Name;
                        columns.Add(columnName);
                        row[columnName] = value;
                    }
                }

                // Optional: flatten related matter info if present
                if (calendarEvent.matter != null)
                {
                    foreach (var matterProp in calendarEvent.matter.GetType().GetProperties())
                    {
                        var matterValue = matterProp.GetValue(calendarEvent.matter);
                        if (matterValue != null)
                        {
                            string columnName = $"Matter_{matterProp.Name}";
                            columns.Add(columnName);
                            row[columnName] = matterValue;
                        }
                    }
                }

                rows.Add(row);
            }

            foreach (var column in columns.OrderBy(c => c, StringComparer.OrdinalIgnoreCase))
            {
                table.Columns.Add(column);
            }

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

    }
}
