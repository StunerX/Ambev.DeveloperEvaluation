namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetPagedSales;

public class GetPagedSalesRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}