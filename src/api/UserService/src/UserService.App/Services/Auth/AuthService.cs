using UserService.App.Common;
using UserService.App.Interfaces;
using UserService.Domain.Entities;
using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces;
using UserService.Domain.ValueObjects;

namespace UserService.App.Services.Auth;

public class AuthService(
    IUserService userService,
    IJwtTokenGenerator jwtTokenGenerator, 
    IUserRepository userRepository,
    IAuthRepository authRepository,
    IPasswordHasher passwordHasher) : IAuthService
{
    private readonly IUserService _userService = userService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IAuthRepository _authRepository = authRepository;

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        try
        {
            var emailValueObject = new Email(email);
            var passwordValueObject = new Password(password);

            var user = await _userRepository.GetByEmailAsync(emailValueObject)
                ?? throw new UserNotFoundException(email);

            if (!_passwordHasher.VerifyPassword(passwordValueObject.Value, user.PasswordHash))
                throw new InvalidCredentialsException();

            var tokenJwt = _jwtTokenGenerator.GenerateToken(user);
            var refreshTokenString = _jwtTokenGenerator.GenerateRefreshToken();

            var refreshToken = new RefreshToken(
                userId: Guid.Parse(user.Id),
                token: refreshTokenString,
                createdAt: DateTime.UtcNow,
                expiresAt: DateTime.UtcNow.AddDays(AppConstants.RefreshTokenDurationInDays));

            await _authRepository.SaveRefreshTokenAsync(refreshToken, CancellationToken.None);

            return new AuthResult(tokenJwt, refreshTokenString);
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidCredentialsException or UserNotFoundException)
        {
            throw new InvalidOperationException("Credenciais inválidas.", ex);
        }
    }

    public async Task<AuthResult> RegisterAsync(string name, string email, string password, string? customerId = null, string? connectedAccountId = null)
    {
        try
        {
            var emailValueObject = new Email(email);
            var passwordValueObject = new Password(password);

            var user = await _userService.CreateUserAsync(name, emailValueObject, passwordValueObject);

            var token = _jwtTokenGenerator.GenerateToken(user);
            var refreshTokenString = _jwtTokenGenerator.GenerateRefreshToken();

            var refreshToken = new RefreshToken(
                userId: Guid.Parse(user.Id),
                token: refreshTokenString,
                createdAt: DateTime.UtcNow,
                expiresAt: DateTime.UtcNow.AddDays(AppConstants.RefreshTokenDurationInDays));

            await _authRepository.SaveRefreshTokenAsync(refreshToken, CancellationToken.None);

            return new AuthResult(token, refreshTokenString);
        }
        catch (Exception ex) when (ex is ArgumentException or UserAlreadyExistsException)
        {
            throw new InvalidOperationException(ex.Message, ex);
        }
    }

    public async Task<AuthResult> RefreshTokenAsync(string oldRefreshToken)
    {
        var existingToken = await _authRepository.GetRefreshTokenAsync(oldRefreshToken, CancellationToken.None);
        if (existingToken == null || existingToken.IsRevoked || existingToken.ExpiresAt < DateTime.UtcNow)
            throw new InvalidOperationException("Refresh token inválido ou expirado.");

        var user = await _userRepository.GetByIdAsync(existingToken.UserId.ToString())
            ?? throw new UserNotFoundException("Usuário não encontrado.");

        var newJwtToken = _jwtTokenGenerator.GenerateToken(user);
        var newRefreshTokenString = _jwtTokenGenerator.GenerateRefreshToken();

        var newRefreshToken = new RefreshToken(
            userId: existingToken.UserId,
            token: newRefreshTokenString,
            createdAt: DateTime.UtcNow,
            expiresAt: DateTime.UtcNow.AddDays(AppConstants.RefreshTokenDurationInDays));

        await _authRepository.SaveRefreshTokenAsync(newRefreshToken, CancellationToken.None);
        await _authRepository.RevokeRefreshTokenAsync(oldRefreshToken, CancellationToken.None);

        return new AuthResult(newJwtToken, newRefreshTokenString);
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        await _authRepository.RevokeRefreshTokenAsync(refreshToken, CancellationToken.None);
    }
}