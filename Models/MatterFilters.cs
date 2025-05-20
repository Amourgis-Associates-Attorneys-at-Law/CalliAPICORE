using System;
using System.Collections.Generic;
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
        /// <param name="matters">An enumerable list of Matter objects.</param>
        /// <param name="status">A string representing status ("open", "closed", or "pending").</param>
        /// <returns></returns>
        public static IEnumerable<Matter> FilterByStatus(this IEnumerable<Matter> matters, string status)
        {
            return matters.Where(m => m.status == status);
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

    }
}
