using UserService.App.Common.DTOs;
using UserService.Domain.Entities;

namespace UserService.App.Common.Results.Mappers;

public static class UserResultMapper
{
    public static UserResult ToUserResult(this User user)
    {
        return new UserResult(
            Id: user.Id,
            Name: user.Name,
            Email: user.Email,
            PhoneNumber: user.PhoneNumber,
            ProfilePic: user.ProfilePic,
            Role: user.Role,
            CreatedAt: user.CreatedAt,
            DeliveryAddresses: [.. user.DeliveryAddresses.Select(address => address.ToDeliveryAddressResult())]);
    }
}