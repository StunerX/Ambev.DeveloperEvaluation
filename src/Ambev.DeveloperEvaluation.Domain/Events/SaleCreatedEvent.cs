namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleCreatedEvent
{
    public Guid SaleId { get; }
    public string Number { get; }
    public Guid CustomerId { get; }
    public string CustomerName { get; }
    public decimal Amount { get; }

    public SaleCreatedEvent(Guid saleId, string number, Guid customerId, string customerName, decimal amount)
    {
        SaleId = saleId;
        Number = number;
        CustomerId = customerId;
        CustomerName = customerName;
        Amount = amount;
    }
}