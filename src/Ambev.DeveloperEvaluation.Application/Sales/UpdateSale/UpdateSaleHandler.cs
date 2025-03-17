using Ambev.DeveloperEvaluation.Application.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateSaleHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var repository = _unitOfWork.Repository<Sale>();
        var sale = await _unitOfWork.Repository<Sale>().FirstOrDefaultAsync(x => x.Id == command.Id, x => x, ["Items"], cancellationToken);

        if (sale == null)
        {
            throw new NotFoundException("Sale not found");
        }

        sale.Update(command.CustomerId, command.CustomerName, command.BranchId, command.BranchName,
            command.Items.Select(x => SaleItem.Create(x.ProductId, x.ProductName, x.Quantity, x.UnitPrice)).ToList());
        
        repository.Update(sale);

         var saleItemRepository = _unitOfWork.Repository<SaleItem>();
        
         foreach (var item in sale.Items)
         {
             if (item.IsCancelled)
             {
                 saleItemRepository.Update(item);
             }
        
             else
             {
                 await saleItemRepository.AddAsync(item, cancellationToken);
             }
         }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}