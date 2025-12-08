using UserService.App.Common.DTOs;
using UserService.Domain.Entities;

namespace UserService.App.Common.Results.Mappers;

public static class DeliveryAdressResultMapper
{
    public static DeliveryAddressResult ToDeliveryAddressResult(this DeliveryAddress deliveryAddress)
    {
        return new DeliveryAddressResult(
            Id: deliveryAddress.Id,
            Street: deliveryAddress.Street,
            Number: deliveryAddress.Number,
            City: deliveryAddress.City,
            State: deliveryAddress.State,
            ZipCode: deliveryAddress.ZipCode);
    }
}