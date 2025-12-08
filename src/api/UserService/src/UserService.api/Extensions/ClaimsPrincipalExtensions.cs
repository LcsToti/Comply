using System;
using System.Security.Claims;

namespace UserService.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);

        var claim = principal.FindFirst(ClaimTypes.NameIdentifier) ?? principal.FindFirst("sub");
            
        return claim == null ? throw new InvalidOperationException("Claim de ID do usuário ('sub' ou 'nameidentifier') não encontrada no token.") : claim.Value;
    }
}