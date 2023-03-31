using System;
using Toolit.Models.Ui;

namespace Toolit.Extensions
{
    public static class TaskStringExtensions
    {
        public static bool ContainsWithComparer(this string source, string query)
        {
            return source != null && query != null && source.IndexOf(query, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }

        public static bool DoesMatchQuery(this TaskUiModel task, string query)
        {
            return task.Title.ContainsWithComparer(query) ||
                   task.Description.ContainsWithComparer(query) ||
                   task.Craft.ContainsWithComparer(query) ||
                   task.City.ContainsWithComparer(query);
        }
    }
}