using Ambev.DeveloperEvaluation.Application.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSaleHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteSaleCommand command, CancellationToken cancellationToken)
    {
        var saleRepository = _unitOfWork.Repository<Sale>();

        var sale = await saleRepository
            .FirstOrDefaultAsync(s => s.Id == command.Id, s => s, ["Items"], cancellationToken);

        if (sale == null)
            throw new NotFoundException("Sale not found.");

        sale.CancelSale(); 
        sale.DeletedAt = DateTime.UtcNow;

        foreach (var item in sale.Items.Where(i => !i.IsCancelled))
        {
            item.Remove();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}