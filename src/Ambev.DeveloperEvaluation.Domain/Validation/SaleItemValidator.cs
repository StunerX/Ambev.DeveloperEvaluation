using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class SaleItemValidator : AbstractValidator<SaleItem>
{
    public SaleItemValidator()
    {

        RuleFor(user => user.ProductId)
            .NotEmpty()
            .WithMessage("Product cannot be empty");
        
        RuleFor(user => user.ProductName)
            .NotEmpty()
            .WithMessage("Product name cannot be empty");
        
        RuleFor(user => user.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(20)
            .WithMessage("Quantity must be less than 20");
    }
}