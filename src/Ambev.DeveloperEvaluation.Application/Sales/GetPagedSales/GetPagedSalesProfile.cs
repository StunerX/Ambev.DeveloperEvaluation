using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetPagedSales;

public class GetPagedSalesProfile : Profile
{
    public GetPagedSalesProfile()
    {
        CreateMap<Sale, GetPagedSalesQueryResult>();
        CreateMap<SaleItem, GetPagedSalesItemQueryResult>();
    }
}