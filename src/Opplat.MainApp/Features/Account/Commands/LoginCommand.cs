using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Finbuckle.MultiTenant.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Opplat.MainApp.Models;

namespace Opplat.MainApp.Features.Account.Commands;

public record LoginCommand(string UserName, string Password) : IRequest<LoginResult?>;

public record LoginResult(string Token, DateTime Expiration, string UserId);

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult?>
{
    private readonly UserManager<Usuario> _userManager;
    private readonly SignInManager<Usuario> _signInManager;
    private readonly IConfiguration _config;
    private readonly IMultiTenantContextAccessor<AppTenantInfo>? _tenantAccessor;

    public LoginCommandHandler(
        UserManager<Usuario> userManager,
        SignInManager<Usuario> signInManager,
        IConfiguration config,
        IMultiTenantContextAccessor<AppTenantInfo>? tenantAccessor = null)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
        _tenantAccessor = tenantAccessor;
    }

    public async Task<LoginResult?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await _signInManager.PasswordSignInAsync(
            request.UserName, request.Password, isPersistent: true, lockoutOnFailure: false);

        if (!result.Succeeded) return null;

        var user = await _userManager.FindByNameAsync(request.UserName);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        var expiration = DateTime.UtcNow.AddDays(1);
        var token = BuildToken(request.UserName, user.Email!, expiration, roles);
        return new LoginResult(token, expiration, user.Id);
    }

    private string BuildToken(string userName, string email, DateTime expiration, IList<string> roles)
    {
        var tenantInfo = _tenantAccessor?.MultiTenantContext?.TenantInfo;

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.UniqueName, userName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim("tenant_id", tenantInfo?.Id ?? "unknown"),
            new Claim("tenant_identifier", tenantInfo?.Identifier ?? "unknown"),
        };

        foreach (var rol in roles)
            claims.Add(new Claim(ClaimTypes.Role, rol));

        var signingKey = (tenantInfo as AppTenantInfo)?.JwtSigningKey
                         ?? _config["Authorization:Password"];
        var key  = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey!));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Authorization:Issuer"],
            audience: _config["Authorization:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: cred);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
