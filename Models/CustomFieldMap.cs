using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalliAPI.Models
{

    public enum CustomField
    {
        PaymentPlanInfo,
        ExpectedPIFDate
    }

    internal class CustomFieldMap
    {
        private static readonly Dictionary<CustomField, long> _fieldIds = new Dictionary<CustomField, long>()
        {
            { CustomField.PaymentPlanInfo, 13582683 },
            { CustomField.ExpectedPIFDate, 13582698 }
        };

        public static long GetId(CustomField field) => _fieldIds[field];

        public static bool TryGetField(long id, out CustomField field)
        {
            field = _fieldIds.FirstOrDefault(kvp => kvp.Value == id).Key;
            return _fieldIds.ContainsValue(id);
        }

    }
}
