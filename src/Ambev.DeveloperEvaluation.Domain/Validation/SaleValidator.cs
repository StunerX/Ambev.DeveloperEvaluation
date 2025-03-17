using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class SaleValidator : AbstractValidator<Sale>
{
    public SaleValidator()
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
            .Must(items => items.Count > 0).WithMessage("At least one sale item is required.");
        
        RuleForEach(x => x.Items).SetValidator(new SaleItemValidator());
    }
}