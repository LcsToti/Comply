using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces;

public interface IAuthRepository
{
    Task SaveRefreshTokenAsync(RefreshToken token, CancellationToken ct);
    Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken ct);
    Task RevokeRefreshTokenAsync(string token, CancellationToken ct);
}
