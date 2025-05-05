
using System.Security.Claims;
using Server.Models;

namespace Server.Services;
public interface ITokenService
    {
        Task<string> GenerateAccessToken(ApplicationUser user);
        Task<RefreshToken> GenerateRefreshToken(string userId);
        Task<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token);
        Task<bool> ValidateRefreshToken(string userId, string refreshToken);
        Task RevokeRefreshToken(string userId, string refreshToken);
    }