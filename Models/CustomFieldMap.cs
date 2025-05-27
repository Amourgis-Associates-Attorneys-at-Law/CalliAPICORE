using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalliAPI.Models
{

    public enum CustomField
    {
        EmergencyCaseType,
        ExpectedPIFDate,
        PaymentPlanInfo
    }

    internal class CustomFieldMap
    {
        private static readonly Dictionary<CustomField, long> _fieldIds = new Dictionary<CustomField, long>()
        {
            { CustomField.EmergencyCaseType, 13584798 },
            { CustomField.ExpectedPIFDate, 13582698 },
            { CustomField.PaymentPlanInfo, 13582683 }
        };

        public static long GetId(CustomField field) => _fieldIds[field];

        public static bool TryGetField(long id, out CustomField field)
        {
            field = _fieldIds.FirstOrDefault(kvp => kvp.Value == id).Key;
            return _fieldIds.ContainsValue(id);
        }

    }
}
