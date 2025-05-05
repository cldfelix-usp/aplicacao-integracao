
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
using Server.Application.DTOs;
using System.Security.Claims;

namespace Server.Controllers;

[Authorize]
[Route("api/v1/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public UserController(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [HttpGet("profileasync")]
    public async Task<IActionResult> GetProfileAsync()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new ResultBaseDto<ApplicationUser>("Usuario nao autorizado!"));

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound(new ResultBaseDto<ApplicationUser>("Usuário não encontrado"));
        }

        // Carregar logins externos
        var externalLogins = await _context.ExternalLogins
            .Where(el => el.UserId == userId)
            .ToListAsync();

        return Ok(new
        {
            id = user.Id,
            email = user.Email,
            userName = user.UserName,
            firstName = user.FirstName,
            lastName = user.LastName,
            profilePictureUrl = user.ProfilePictureUrl,
            externalLogins = externalLogins.Select(el => new
            {
                provider = el.LoginProvider,
                displayName = el.ProviderDisplayName
            })
        });
    }

    [HttpPut("profileasync")]
    public async Task<IActionResult> UpdateProfileAsync([FromBody] UpdateProfileRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new ResultBaseDto<ProfileDto>("Usuario nao autorizado!"));

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound(new ResultBaseDto<ProfileDto>("Usuário não encontrado"));

        // Atualizar os campos do perfil
        user.FirstName = request.FirstName ?? user.FirstName;
        user.LastName = request.LastName ?? user.LastName;
        user.ProfilePictureUrl = request.ProfilePictureUrl ?? user.ProfilePictureUrl;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(new ResultBaseDto<ProfileDto>(result.Errors.ToString()));

        return Ok(new ProfileDto()
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ProfilePictureUrl = user.ProfilePictureUrl
        });
    }
}