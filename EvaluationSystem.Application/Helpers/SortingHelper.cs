using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.Helpers
{
    public static class SortingHelper
    {
        public static IQueryable<T> ApplySorting<T>( IQueryable<T> query, string? sortBy, bool descending, Dictionary<string, Func<IQueryable<T>, IQueryable<T>>> sortOptions) where T : class
        {
            // If no sortBy specified, sort by Id ascending (default)
            if (string.IsNullOrWhiteSpace(sortBy))
                return query.OrderBy(x => EF.Property<int>(x, "Id"));

            // Convert to lowercase for case-insensitive matching
            var key = sortBy.ToLower();

            // Check if the sort field exists in our options
            if (!sortOptions.ContainsKey(key))
                return query.OrderBy(x => EF.Property<int>(x, "Id")); // Default sorting

            // Apply the sorting function from our options
            return sortOptions[key](query);
        }
    }
}
