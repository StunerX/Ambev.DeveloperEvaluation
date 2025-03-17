using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        RuleFor(x => x.Number)
            .NotEmpty().WithMessage("Sale number is required.")
            .MaximumLength(50).WithMessage("Sale number must be at most 50 characters.");
        
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required.");
        
        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("Customer name is required.")
            .MaximumLength(200).WithMessage("Customer name must be at most 200 characters.");
        
        RuleFor(x => x.BranchId)
            .NotEmpty().WithMessage("BranchId is required.");
        
        RuleFor(x => x.BranchName)
            .NotEmpty().WithMessage("Branch name is required.")
            .MaximumLength(200).WithMessage("Branch name must be at most 200 characters.");
        
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one sale item is required.")
            .Must(items => items.Count > 0).WithMessage("At least one sale item is required.");
        
        RuleForEach(x => x.Items).SetValidator(new CreateSaleItemCommandValidator());
    }  
}