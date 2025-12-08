using UserService.Domain.Entities;
using UserService.Domain.Enums;

namespace UserService.Domain.Factories;

public class UserFactory
{
    public static User Create(string name, string email, string passwordHash)
    {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nome inválido", nameof(name));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email inválido", nameof(email));
            if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentException("PasswordHash inválido", nameof(passwordHash));

            var user = new User(null, name, email, passwordHash, Role.User, DateTime.UtcNow, new List<DeliveryAddress>(), new List<string>());
            return user;
    }
    
    public static User Load(
        string id,
        string name,
        string email,
        string passwordHash,
        Role role,
        DateTime createdAt,
        List<DeliveryAddress> deliveryAddresses,
        List<string> watchList,
        string? phoneNumber = null,
        string? profilePic = null
    )
    {
        var user = new User(id, name, email, passwordHash, role, createdAt, deliveryAddresses, watchList);
        
        if (!string.IsNullOrWhiteSpace(phoneNumber))
            user.ChangePhoneNumber(phoneNumber);
        
        if (!string.IsNullOrWhiteSpace(profilePic))
            user.UpdateProfilePicture(profilePic);

        return user;
    }
}