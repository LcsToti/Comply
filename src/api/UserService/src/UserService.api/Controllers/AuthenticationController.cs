using Microsoft.AspNetCore.Mvc;
using UserService.App.Common;
using UserService.App.Interfaces;
using UserService.Contracts.Authentication;
using UserService.Domain.Exceptions;

namespace UserService.Api.Controllers;

[ApiController]
[Route("api/v1/Auth")]
public class AuthenticationController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        try
        {
            var result = await _authService.RegisterAsync(request.Name, request.Email, request.Password);

            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(AppConstants.RefreshTokenDurationInDays)
            });

            var response = new AuthenticationResponse(result.TokenJwt);

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(request.Email, request.Password);

            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(AppConstants.RefreshTokenDurationInDays)
            });

            var response = new AuthenticationResponse(result.TokenJwt);

            return Ok(response);
        }
        catch (UserNotFoundException)
        {
            return BadRequest("Credenciais inválidas");
        }
        catch (InvalidCredentialsException)
        {
            return BadRequest("Credenciais inválidas");
        }
        catch (InvalidOperationException)
        {
            return BadRequest("Credenciais inválidas");
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var oldRefreshToken))
            return Unauthorized(new { message = "Refresh token não encontrado." });

        try
        {
            var result = await _authService.RefreshTokenAsync(oldRefreshToken);

            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(AppConstants.RefreshTokenDurationInDays)
            });

            var response = new AuthenticationResponse(result.TokenJwt);

            return Ok(response);
        }
        catch (InvalidOperationException)
        {
            return Unauthorized(new { message = "Refresh token inválido ou expirado." });
        }
        catch (UserNotFoundException)
        {
            return Unauthorized(new { message = "Usuário não encontrado." });
        }
        catch
        {
            return StatusCode(500, new { message = "Erro interno." });
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        if (Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            await _authService.RevokeRefreshTokenAsync(refreshToken);

        Response.Cookies.Append("refreshToken", "", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(-1)
        });

        return Ok(new { message = "Logout realizado." });
    }

}