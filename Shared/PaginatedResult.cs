namespace Shared
{
    public record PaginatedResult<TData>(
        int PageSize,
        int PageIndex,
        int TotalCount,
        IEnumerable<TData> Data
    );
}
