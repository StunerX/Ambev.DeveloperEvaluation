using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.GetPagedSales;
using Ambev.DeveloperEvaluation.WebApi.Common;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetPagedSales;

public class GetPagedSalesProfile : Profile
{
    public GetPagedSalesProfile()
    {
        CreateMap<GetPagedSalesRequest, GetPagedSalesQuery>();
        CreateMap<GetPagedSalesQueryResult, GetPagedSalesResponse>();
        CreateMap<GetPagedSalesItemQueryResult, GetPagedSalesItemResponse>();
        CreateMap<PagedResult<GetPagedSalesQueryResult>, PagedResponse<GetPagedSalesResponse>>()
            .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Items));
    }
}