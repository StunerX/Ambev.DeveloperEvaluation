using Ambev.DeveloperEvaluation.Application.Common;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetPagedSales;

public class GetPagedSalesQuery : IRequest<PagedResult<GetPagedSalesQueryResult>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}