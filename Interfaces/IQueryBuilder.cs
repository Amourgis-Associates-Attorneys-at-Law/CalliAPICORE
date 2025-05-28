using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalliAPI.Interfaces
{
    public interface IQueryBuilder<T>
    {
        /// <summary>
        /// The list of fields selected by the user, formatted for the Clio API.
        /// </summary>
        public abstract string SelectedFields { get; }

        /// <summary>
        /// A list of filters to apply to the result stream.
        /// </summary>
        public abstract List<Func<IAsyncEnumerable<T>, IAsyncEnumerable<T>>> SelectedFilters { get; }

        /// <summary>
        /// Optional query parameters to include in the API request.
        /// </summary>
        public virtual Dictionary<string, string> QueryParameters => new();

        /// <summary>
        /// Optional description of the query for logging or display.
        /// </summary>
        public virtual string QueryDescription =>
     $"Fields: {SelectedFields}, Filters: {SelectedFilters.Count}";
    }
}
