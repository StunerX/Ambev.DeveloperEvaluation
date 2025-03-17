using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetPagedSales;

public class GetPagedSalesQueryHandler : IRequestHandler<GetPagedSalesQuery, PagedResult<GetPagedSalesQueryResult>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPagedSalesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<PagedResult<GetPagedSalesQueryResult>> Handle(GetPagedSalesQuery query, CancellationToken cancellationToken)
    {
        var saleRepository = _unitOfWork.Repository<Sale>();

        var entityQuery = saleRepository.CreateQuery(x => x.IsCancelled == false);
        var sales = await saleRepository.FindAsPaginatedOrderAsync(query.Page, query.PageSize, string.Empty, false, x => _mapper.Map<GetPagedSalesQueryResult>(x), null, entityQuery, 
            ["Items"], cancellationToken);

        var salesCount = await saleRepository.CountAsync(entityQuery, cancellationToken);

        return PagedResult<GetPagedSalesQueryResult>.Create(sales.ToList(), query.Page, query.PageSize, salesCount);
    }
}