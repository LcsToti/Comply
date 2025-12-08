using UserService.Domain.Enums;

namespace UserService.Domain.Entities
{
    public class User
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string? PhoneNumber { get; private set; }
        public string? ProfilePic { get; private set; }
        public Role Role { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private readonly List<DeliveryAddress> _deliveryAddresses;
        public IReadOnlyList<DeliveryAddress> DeliveryAddresses => _deliveryAddresses.AsReadOnly();

        private readonly List<string> _watchList;
        public IReadOnlyList<string> WatchList => _watchList.AsReadOnly();

        internal User(string? id, string name, string email, string passwordHash, Role role, DateTime createdAt, List<DeliveryAddress> deliveryAddresses, List<string> watchList)
        {
            Id = id ?? Guid.NewGuid().ToString();
            Name = name;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
            CreatedAt = createdAt;
            _deliveryAddresses = deliveryAddresses;
            _watchList = watchList;     
        }

        #region Profile Updates

        public void ChangeName(string? newName)
        {
            Name = string.IsNullOrWhiteSpace(newName) ? Name : newName;       
        }

        public void ChangePhoneNumber(string? newPhoneNumber)
        {
            PhoneNumber = string.IsNullOrWhiteSpace(newPhoneNumber) ? PhoneNumber : newPhoneNumber;
        }

        public void UpdateProfilePicture(string? newProfilePicUrl)
        {
            ProfilePic = string.IsNullOrWhiteSpace(newProfilePicUrl) ? ProfilePic : newProfilePicUrl;
        }
        
        public void ChangePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentException("Hash da senha não pode ser vazio.", nameof(newPasswordHash));

            PasswordHash = newPasswordHash;
        }

        #endregion

        #region DeliveryAddresses

        public void AddDeliveryAddress(DeliveryAddress address)
        {
            ArgumentNullException.ThrowIfNull(address);
            
            if (_deliveryAddresses.Contains(address)) return;
            _deliveryAddresses.Add(address);
        }
        public void UpdateDeliveryAddress(DeliveryAddress existing, DeliveryAddress updated)
        {
            var idx = _deliveryAddresses.FindIndex(a => a.Equals(existing));
            if (idx == -1) throw new InvalidOperationException("Endereço não encontrado.");
            _deliveryAddresses[idx] = updated;
        }
        public void RemoveDeliveryAddress(DeliveryAddress address)
        {
            if (!_deliveryAddresses.Remove(address))
                throw new InvalidOperationException("Endereço não encontrado.");
        }

        #endregion
        
        #region Roles
        public void ChangeRole(Role newRole)
        {
            Role = newRole;
        }

        public bool HasRole(Role role)
        {
            return Role == role;
        }

        public bool IsAdmin()
        {
            return Role == Role.Admin;
        }

        public bool IsModerator()
        {
            return Role == Role.Moderator;
        }

        public bool IsUser()
        {
            return Role == Role.User;
        }
        #endregion

        #region WatchList

        public void AddToWatchList(string productId)
        {
            if (string.IsNullOrWhiteSpace(productId)) 
                throw new ArgumentException("ID do produto inválido.", nameof(productId));
            if (!_watchList.Contains(productId))
                _watchList.Add(productId);
        }

        public void RemoveFromWatchList(string productId)
        {
            _watchList.Remove(productId);
        }

        #endregion
    }
}
