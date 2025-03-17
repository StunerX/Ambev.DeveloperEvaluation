using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IEventPublisher _eventPublisher;

    public CreateSaleHandler(IUnitOfWork unitOfWork, IMapper mapper, IEventPublisher eventPublisher)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _eventPublisher = eventPublisher;
    }
    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var sale = Sale.Create(command.Number, command.CustomerId, command.CustomerName, command.BranchId,
            command.BranchName, command.Items.ConvertAll(item => SaleItem.Create(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice)));
        
        await _unitOfWork.Repository<Sale>().AddAsync(sale, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var saleCreatedEvent = new SaleCreatedEvent(sale.Id, sale.Number, sale.CustomerId, sale.CustomerName, sale.Amount);
        await _eventPublisher.PublishAsync(saleCreatedEvent, cancellationToken);
        
        return _mapper.Map<CreateSaleResult>(sale);
    }
}