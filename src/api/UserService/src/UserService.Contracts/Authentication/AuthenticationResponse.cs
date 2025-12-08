using System;
using UserService.Domain.Entities;

namespace UserService.Contracts.Authentication;

public record AuthenticationResponse(string Token);
