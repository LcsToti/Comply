using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using UserService.Domain.Entities;
using UserService.Domain.Enums;
using UserService.Domain.Factories;

namespace UserService.Infra.Persistence.Documents;

public class UserDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)] 
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ProfilePic { get; set; }
    public string Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<DeliveryAddress> DeliveryAddresses { get; set; } = new();
    public List<string> WatchList { get; set; } = new();

    
    public static UserDocument FromDomain(User user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        PasswordHash = user.PasswordHash,
        PhoneNumber = user.PhoneNumber,
        ProfilePic = user.ProfilePic,
        Role = user.Role.ToString(),
        CreatedAt = user.CreatedAt,    
        DeliveryAddresses = user.DeliveryAddresses.ToList(),
        WatchList = user.WatchList.ToList()
    };
    public User ToDomain()
    {
        var roleEnum = Enum.Parse<Role>(Role);

        return UserFactory.Load(
            id: Id,
            name: Name,
            email: Email,
            passwordHash: PasswordHash,
            role: roleEnum,
            createdAt: CreatedAt,    
            deliveryAddresses: DeliveryAddresses,
            watchList: WatchList,
            phoneNumber: PhoneNumber,
            profilePic: ProfilePic   
        );
    }
}