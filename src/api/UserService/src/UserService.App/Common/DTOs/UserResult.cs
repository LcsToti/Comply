using UserService.Domain.Enums;

namespace UserService.App.Common.DTOs;

public record UserResult(
        string Id,
        string Name,
        string Email,
        string? PhoneNumber,
        string? ProfilePic,
        Role Role,
        DateTime CreatedAt,
        List<DeliveryAddressResult> DeliveryAddresses);