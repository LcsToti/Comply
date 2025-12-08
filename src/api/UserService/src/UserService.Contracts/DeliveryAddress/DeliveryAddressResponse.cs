namespace UserService.Contracts.DeliveryAddress
{
    public record DeliveryAddressResponse(
        string Street,
        string Number,
        string City,
        string State,
        string ZipCode
    );
}
