using UserService.Domain.Entities;

namespace UserService.App.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
}



