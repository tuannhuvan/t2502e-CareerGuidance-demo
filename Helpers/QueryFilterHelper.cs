using CareerGuidance.Models;

namespace CareerGuidance.Helpers;

public static class QueryFilterHelper
{
    public static IQueryable<T> ApplyDateRange<T>(IQueryable<T> query, string? dateRange)
        where T : BaseEntity
    {
        if (string.IsNullOrWhiteSpace(dateRange))
        {
            return query;
        }

        var dates = dateRange.Split(" - ", StringSplitOptions.TrimEntries);
        if (dates.Length != 2)
        {
            return query;
        }

        if (!DateTime.TryParseExact(dates[0], "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out var startDate))
        {
            return query;
        }

        if (!DateTime.TryParseExact(dates[1], "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out var endDate))
        {
            return query;
        }

        endDate = endDate.AddDays(1);
        return query.Where(x => x.CreatedAt >= startDate && x.CreatedAt < endDate);
    }
}
