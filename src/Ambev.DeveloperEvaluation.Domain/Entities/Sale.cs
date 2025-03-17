using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale : BaseEntity
{
    public string Number { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; }
    public Guid BranchId { get; set; }
    public string BranchName { get; set; }
    public decimal Amount { get; set; }
    public List<SaleItem> Items { get; set; }
    public bool IsCancelled { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    protected Sale() {}

    private Sale(string number, Guid customerId, string customerName, Guid branchId, string branchName)
    {
        Id = Guid.NewGuid();
        Number = number;
        CustomerId = customerId;
        CustomerName = customerName;
        BranchId = branchId;
        BranchName = branchName;
        Items = new List<SaleItem>();
        CreatedAt = DateTime.UtcNow;

        CalculateTotalAmount();
    }

    public static Sale Create(string number, Guid customerId, string customerName, Guid branchId, string branchName, List<SaleItem> items)
    {
        var sale = new Sale(number, customerId, customerName, branchId, branchName);

        foreach (var saleItem in items)
        {
            sale.AddItem(saleItem);
        }

        sale.Validate();
        
        return sale;
    }

    public void Update(Guid customerId, string customerName, Guid branchId, string branchName, List<SaleItem> items)
    {
        CustomerId = customerId;
        CustomerName = customerName;
        BranchId = branchId;
        BranchName = branchName;

        if (items.Count == 0) return;
        
        RemoveAllItems();

        foreach (var item in items)
        {
            AddItem(item);
        }

        Validate();
    }
    
    public void AddItem(SaleItem item)
    {
        Items.Add(item);
        CalculateTotalAmount();
    }

    public void RemoveItem(SaleItem item)
    {
        Items.Remove(item);
        CalculateTotalAmount();
    }

    public void CalculateTotalAmount()
    {
        Amount = Items.Where(x => !x.IsCancelled).Sum(item => item.Amount);
    }

    public void CancelSale()
    {
        IsCancelled = true;
        DeletedAt = DateTime.UtcNow;
    }

    private void Validate()
    {
        var validator = new SaleValidator();
        var validationResult = validator.Validate(this);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new DomainException(errors);
        }
    }

    private void RemoveAllItems()
    {
        foreach (var item in Items)
        {
            item.Remove();
        }
    }
    
}