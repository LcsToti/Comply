using UserService.App.Common.DTOs;
using UserService.Domain.Entities;
using UserService.Domain.ValueObjects;

namespace UserService.App.Interfaces
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(string name, Email email, Password password);
        Task<UserResult?> GetUserByEmailAsync(Email email);
        Task<UserResult?> GetUserAsync(string userId);
        Task ChangeNameAsync(string email, string? newName);
        Task ChangePhoneNumberAsync(string userId, string? newPhoneNumber);
        Task UpdateProfilePictureAsync(string userId, string? newProfilePicUrl);
        Task ChangePasswordAsync(string userId, Password newPassword);
        Task AddDeliveryAddressAsync(string userId, DeliveryAddress address);
        Task UpdateDeliveryAddressAsync(string userId, string addressId, DeliveryAddress address);
        Task RemoveDeliveryAddressAsync(string userId, string addressId);
        Task<DeliveryAddress?> GetDeliveryAddressByIdAsync(string userId, string addressId);
        Task<IEnumerable<DeliveryAddress>> GetDeliveryAddressesAsync(string userId);
        Task<long> GetAllUsersCount();
    }
}
