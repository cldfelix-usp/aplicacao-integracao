
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
using Server.Services;
using System.Security.Claims;

namespace Server.Controllers;

[Route("api/v1/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        ApplicationDbContext context,
        ILogger<AuthController> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _tokenService = tokenService;
        _context = context;
        _logger = logger;
    }

    [HttpGet("login")]
    public IActionResult Login(string returnUrl = null)
    {
        // Esta é a página para a qual o middleware de autenticação redirecionará
        // quando o login for necessário.
        return Ok(new { message = "Por favor, faça login para continuar." });
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok(new { message = "Logout realizado com sucesso." });
    }

    // Iniciar login com Google
    [HttpGet("google-login")]
    public IActionResult GoogleLogin()
    {
        var redirectUrl = Url.Action("ExternalLoginCallback", "Auth", new { ReturnUrl = "/" });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
        return Challenge(properties, "Google");
    }

    // Iniciar login com Facebook
    [HttpGet("facebook-login")]
    public IActionResult FacebookLogin()
    {
        var redirectUrl = Url.Action("ExternalLoginCallback", "Auth", new { ReturnUrl = "/" });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", redirectUrl);
        return Challenge(properties, "Facebook");
    }

    // Iniciar login com GitHub
    [HttpGet("github-login")]
    public IActionResult GitHubLogin()
    {
        var redirectUrl = Url.Action("ExternalLoginCallback", "Auth", new { ReturnUrl = "/" });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties("GitHub", redirectUrl);
        return Challenge(properties, "GitHub");
    }

    // Callback comum para todos os provedores externos
    [HttpGet("external-login-callback")]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
    {
        returnUrl = returnUrl ?? Url.Content("~/");

        if (remoteError != null)
        {
            return BadRequest($"Erro do provedor externo: {remoteError}");
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return BadRequest("Erro ao carregar informações de login externo.");
        }

        // Fazer login se o usuário já tiver uma conta
        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

        if (result.Succeeded)
        {
            _logger.LogInformation("{Name} logado com {LoginProvider}.", info.Principal.Identity.Name, info.LoginProvider);

            // Obter o usuário atual
            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            // Gerar tokens JWT
            var accessToken = await _tokenService.GenerateAccessToken(user);
            var refreshToken = await _tokenService.GenerateRefreshToken(user.Id);
            // var expiration = DateTime.Now.AddMinutes(Convert.ToDouble(
            //     Configuration["JwtSettings:AccessTokenExpiryInMinutes"]));

            return Ok(new
            {
                accessToken,
                refreshToken = refreshToken.Token,
                expiration = 180,
                user = new
                {
                    id = user.Id,
                    email = user.Email,
                    userName = user.UserName,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    profilePictureUrl = user.ProfilePictureUrl
                }
            });
        }

        // Se o usuário não tem uma conta, então criar uma
        if (result.IsNotAllowed || result.IsLockedOut)
        {
            return BadRequest("Sua conta está bloqueada ou não tem permissão para login.");
        }
        else
        {
            // Criar um novo usuário
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
                LastName = info.Principal.FindFirstValue(ClaimTypes.Surname),
                ProfilePictureUrl = info.Principal.FindFirstValue("picture")
            };

            var createResult = await _userManager.CreateAsync(user);
            if (createResult.Succeeded)
            {
                createResult = await _userManager.AddLoginAsync(user, info);
                if (createResult.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("Usuário criado com {LoginProvider}.", info.LoginProvider);

                    // Armazenar informações de login externo
                    var externalLogin = new ExternalLogin
                    {
                        LoginProvider = info.LoginProvider,
                        ProviderKey = info.ProviderKey,
                        ProviderDisplayName = info.ProviderDisplayName,
                        UserId = user.Id
                    };
                    await _context.ExternalLogins.AddAsync(externalLogin);
                    await _context.SaveChangesAsync();

                    // Gerar tokens JWT
                    var accessToken = await _tokenService.GenerateAccessToken(user);
                    var refreshToken = await _tokenService.GenerateRefreshToken(user.Id);
                    // var expiration = DateTime.Now.AddMinutes(Convert.ToDouble(
                    //     Configuration["JwtSettings:AccessTokenExpiryInMinutes"]));

                    return Ok(new
                    {
                        accessToken,
                        refreshToken = refreshToken.Token,
                        expiration = 180,
                        user = new
                        {
                            id = user.Id,
                            email = user.Email,
                            userName = user.UserName,
                            firstName = user.FirstName,
                            lastName = user.LastName,
                            profilePictureUrl = user.ProfilePictureUrl
                        }
                    });
                }
            }

            foreach (var error in createResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest(ModelState);
        }
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.AccessToken) || string.IsNullOrEmpty(request.RefreshToken))
        {
            return BadRequest("Token inválido");
        }

        var principal = await _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("Token inválido");
        }

        var isValid = await _tokenService.ValidateRefreshToken(userId, request.RefreshToken);
        if (!isValid)
        {
            return BadRequest("Token de atualização inválido ou expirado");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return BadRequest("Usuário não encontrado");
        }

        // Gerar novos tokens
        var newAccessToken = await _tokenService.GenerateAccessToken(user);
        var newRefreshToken = await _tokenService.GenerateRefreshToken(userId);
        // var expiration = DateTime.Now.AddMinutes(Convert.ToDouble(
        //     Configuration["JwtSettings:AccessTokenExpiryInMinutes"]));
        

        // Revogar o token de atualização antigo
        await _tokenService.RevokeRefreshToken(userId, request.RefreshToken);

        return Ok(new
        {
            accessToken = newAccessToken,
            refreshToken = newRefreshToken.Token,
            expiration = 180,
        });
    }

    [Authorize]
    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest request)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
        {
            return BadRequest("Token de atualização é obrigatório");
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        await _tokenService.RevokeRefreshToken(userId, request.RefreshToken);
        return Ok(new { message = "Token revogado com sucesso" });
    }
}

public class UpdateProfileRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ProfilePictureUrl { get; set; }
}

public class RefreshTokenRequest
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}

public class RevokeTokenRequest
{
    public string RefreshToken { get; set; }
}
