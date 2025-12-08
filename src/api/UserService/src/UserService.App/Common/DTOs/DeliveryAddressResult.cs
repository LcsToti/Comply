namespace UserService.App.Common.DTOs;

public record DeliveryAddressResult(
     string Id,
     string Street,
     string Number,
     string City,
     string State,
     string ZipCode);
