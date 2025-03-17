namespace Ambev.DeveloperEvaluation.WebApi.Common;

public class PagedResponse<T>
{
    public IEnumerable<T> Data { get; set; } = [];
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public int PageSize { get; set; }
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
    public bool Success { get; set; } = true;
    public string? Message { get; set; }
}