using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleItem : BaseEntity
{
    public Guid SaleId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal Amount { get; set; }
    public bool IsCancelled { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    protected SaleItem() { }

    private SaleItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Amount = unitPrice * quantity;
        
        ApplyDiscount();
        CalculateAmount();
    }

    public static SaleItem Create(Guid productId, string productName, int quantity, decimal price)
    {
        var saleItem = new SaleItem(productId, productName, quantity, price);
        
        var validator = new SaleItemValidator();
        var validationResult = validator.Validate(saleItem);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new DomainException(errors);
        }

        return saleItem;
    }

    public void CalculateAmount()
    {
        Amount = UnitPrice * Quantity - Discount;
    }

    public void ApplyDiscount()
    {
        if (Quantity is >= 4 and < 10)
        {
            Discount = Amount * 0.10m;
        }

        if (Quantity is >= 10 and <= 20)
        {
            Discount = Amount * 0.20m;
        }
    }
    
    public ValidationResultDetail Validate()
    {
        var validator = new SaleItemValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }

    public void Remove()
    {
        IsCancelled = true;
        DeletedAt = DateTime.UtcNow;
    }
}