using MassTransit;
using Shared.Contracts.Messages.UserService;
using UserService.App.Common.DTOs;
using UserService.App.Common.Results.Mappers;
using UserService.App.Interfaces;
using UserService.Domain.Entities;
using UserService.Domain.Exceptions;
using UserService.Domain.Factories;
using UserService.Domain.Interfaces;
using UserService.Domain.ValueObjects;

namespace UserService.App.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IPublishEndpoint _publishEndpoint;

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher, IPublishEndpoint publishEndpoint)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));       
        }

        public async Task<User> CreateUserAsync(string name, Email email, Password password)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome é obrigatório.", nameof(name));

            var existingUser = await _userRepository.GetByEmailAsync(email.Value);
            if (existingUser != null)
                throw new UserAlreadyExistsException(email.Value);

            var passwordHash = _passwordHasher.HashPassword(password.Value);

            var user = UserFactory.Create(name, email.Value, passwordHash);

            await _userRepository.AddAsync(user);

            await _publishEndpoint.Publish(new CreatedUserMessage(user.Email, user.Name, Guid.Parse(user.Id)), CancellationToken.None);
            return user;
        }
        public async Task<UserResult?> GetUserByEmailAsync(Email email)
        {
            var user = await _userRepository.GetByEmailAsync(email.Value);
            
            if (user == null)
                throw new UserNotFoundException(email.Value);

            return user.ToUserResult();
        }

        public async Task<UserResult?> GetUserAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            
            if (user == null)
                throw new UserNotFoundException(userId);

            return user.ToUserResult();
        }

        public async Task ChangeNameAsync(string userId, string? newName)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);
            
            user.ChangeName(newName);
            await _userRepository.UpdateAsync(user);
        }
        public async Task ChangePhoneNumberAsync(string userId, string? newPhoneNumber)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);
            user.ChangePhoneNumber(newPhoneNumber);
            await _userRepository.UpdateAsync(user);
        }
        public async Task UpdateProfilePictureAsync(string userId, string? newProfilePicUrl)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);
            user.UpdateProfilePicture(newProfilePicUrl);
            await _userRepository.UpdateAsync(user);
        }
        public async Task ChangePasswordAsync(string email, Password newPassword)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                throw new UserNotFoundException(email);
            
            var newPasswordHash = _passwordHasher.HashPassword(newPassword.Value);
            user.ChangePassword(newPasswordHash);
            await _userRepository.UpdateAsync(user);
        }
        public async Task AddDeliveryAddressAsync(string userId, DeliveryAddress address)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId, byId: true);
            
            user.AddDeliveryAddress(address);
            await _userRepository.UpdateAsync(user);
        }
        public async Task UpdateDeliveryAddressAsync(string userId, string addressId, DeliveryAddress address)
        {
            var success = await _userRepository.UpdateDeliveryAddressAsync(userId, addressId, address);
    
            if (!success)
            {
                throw new InvalidOperationException("Endereço não encontrado ou não pôde ser atualizado.");
            }
        }
        public async Task RemoveDeliveryAddressAsync(string userId, string addressId)
        {
            var success = await _userRepository.RemoveDeliveryAddressAsync(userId, addressId);

            if (!success)
            {
                throw new InvalidOperationException("Endereço não encontrado ou não pôde ser removido.");
            }
        }
        public async Task<DeliveryAddress?> GetDeliveryAddressByIdAsync(string userId, string addressId)
        {
            var address = await _userRepository.GetDeliveryAddressByIdAsync(userId, addressId);
            if (address is null)
            {
                throw new KeyNotFoundException("Endereço não encontrado.");
            }
            return address;
        }
        public async Task<IEnumerable<DeliveryAddress>> GetDeliveryAddressesAsync(string userId)
        {
            var addresses = await _userRepository.GetDeliveryAddressesAsync(userId);
            return addresses;
        }

        public async Task<long> GetAllUsersCount()
        {
            var count = await _userRepository.GetAllUsersCountAsync();
            return count;
        }

        public async Task AddToWatchListAsync(string userId, string productId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);
            user.AddToWatchList(productId);
            await _userRepository.UpdateAsync(user);
        }
        public async Task RemoveFromWatchListAsync(string userId, string productId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);
            user.RemoveFromWatchList(productId);
            await _userRepository.UpdateAsync(user);
        }
    }
}
