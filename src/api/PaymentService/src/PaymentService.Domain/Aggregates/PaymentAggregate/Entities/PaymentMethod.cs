namespace Payments.Domain.Aggregates.PaymentAggregate.Entities;

public class PaymentMethod
{
    public string Id { get; private set; }
    public string Type { get; private set; }
    public string Last4 { get; private set; }
    public string Brand { get; private set; }
    
    internal PaymentMethod(string id, string type, string last4, string brand)
    {
        Id = id;
        Type = type;
        Last4 = last4;
        Brand = brand;
    }
}