using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Server.Models;
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ProfilePictureUrl { get; set; }
    public List<RefreshToken> RefreshTokens { get; set; } = [];
    public List<ExternalLogin> ExternalLogins { get; set; } = [];
}
