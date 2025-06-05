using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalliAPI.Models
{
    /// <summary>
    /// A representation of a Clio calendar (not the events on it, which are ClioCalendarEvent items).
    /// </summary>
    public class ClioCalendar
    {
        /// <summary>
        /// The unique identifier for the calendar.
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// The name of the calendar.
        /// </summary>
        public string? name { get; set; }

        /// <summary>
        /// The permission level for the calendar ("owner", "editor", "viewer", etc.).
        /// </summary>
        public string? permission { get; set; }

        /// <summary>
        /// A boolean that reflects if the current user can see the items on the calendar.
        /// </summary>
        public bool? visible { get; set; }
    }

}
