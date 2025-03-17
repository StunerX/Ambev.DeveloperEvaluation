using Ambev.DeveloperEvaluation.Application.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSaleById;

public class GetSaleByIdQueryHandler : IRequestHandler<GetSaleByIdQuery, GetSaleByIdQueryResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetSaleByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<GetSaleByIdQueryResult> Handle(GetSaleByIdQuery query, CancellationToken cancellationToken)
    {
        var sale = await _unitOfWork.Repository<Sale>().FirstOrDefaultAsync(x => x.Id == query.Id && x.IsCancelled == false, x => x, ["Items"], cancellationToken);

        if (sale == null)
        {
            throw new NotFoundException($"Sale with ID {query.Id} not found.");
        }

        return _mapper.Map<GetSaleByIdQueryResult>(sale);
    }
}