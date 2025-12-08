namespace UserService.App.Services.Auth;

public record AuthResult(string TokenJwt, string RefreshToken);
