using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace WeeControl.SharedKernel.Interfaces;

public interface IJwtService
{
    string GenerateToken(SecurityTokenDescriptor descriptor);

    ClaimsPrincipal ExtractClaimPrincipal(string token);
    ClaimsPrincipal ExtractClaimPrincipal(TokenValidationParameters parameters, string token);
}