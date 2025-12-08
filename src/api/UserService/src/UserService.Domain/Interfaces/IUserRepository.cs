using System;
using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(string id);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task<bool> UpdateDeliveryAddressAsync(string userId, string addressId, DeliveryAddress address);
        Task<bool> RemoveDeliveryAddressAsync(string userId, string addressId);
        Task<DeliveryAddress?> GetDeliveryAddressByIdAsync(string userId, string addressId);
        Task<IEnumerable<DeliveryAddress>> GetDeliveryAddressesAsync(string userId);
        Task<long> GetAllUsersCountAsync();
    }
}