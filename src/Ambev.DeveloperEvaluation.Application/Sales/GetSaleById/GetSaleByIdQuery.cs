using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSaleById;

public class GetSaleByIdQuery : IRequest<GetSaleByIdQueryResult>
{
    public Guid Id { get; }

    public GetSaleByIdQuery(Guid id)
    {
        Id = id;
    }
}