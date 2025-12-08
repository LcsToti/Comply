using System;
using UserService.Domain.Entities;

namespace UserService.Contracts.Profile
{
    public record UserProfileResponse(
        string Name,
        string Email,
        string? PhoneNumber,
        string? ProfilePic,
        Domain.Enums.Role Role,
        DateTime CreatedAt
    );
}
