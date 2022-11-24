using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Opplat.MainApp.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Opplat.MainApp.Data;
using Opplat.MainApp.Models;

namespace Opplat.MainApp.Controllers;

[Route("auth/[controller]")]
[ApiController]
[Authorize]
public class AccountController : ControllerBase
{
    private readonly UserManager<Usuario> _userManager;
    private readonly SignInManager<Usuario> _signInManager;
    private readonly ILogger _logger;
    private readonly DbContext _db;
    private readonly IConfiguration _config;

    public AccountController(
        UserManager<Usuario> userManager,
        SignInManager<Usuario> signInManager, IConfiguration config,
        ILogger<AccountController> logger, OpplatDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _db = context;
        _config = config;
    }

    /// <summary>
    /// Devuelve el listado de usuarios
    /// </summary>
    /// <returns></returns>
    [HttpGet("user-list")]
    public async Task<IEnumerable<AccountDto>> UserList()
    {
        var result = await _db.Set<Usuario>().Select(u => new AccountDto
        {
            UserId = u.Id,
            Name = u.Nombres,
            LastName = u.Apellidos,
            Username = u.UserName,
            Email = u.Email,
            Active = u.Activo,
            Roles = (_userManager.GetRolesAsync(u)).Result.ToList()
        }).ToListAsync();
        return result;
    }

    /// <summary>
    /// Devuelve los datos del usuario
    /// </summary>
    /// <returns></returns>
    [HttpGet("profile/{name}")]
    public async Task<AccountDto> Profile(string name)
    {
        var user = await _userManager.FindByNameAsync(name);
        if (user == null)
        {
            return null;
        }
        var roles = await _userManager.GetRolesAsync(user);
        return new AccountDto
        {
            UserId = user.Id,
            Name = user.Nombres,
            LastName = user.Apellidos,
            Username = user.UserName,
            Email = user.Email,
            Active = user.Activo,
            Roles = roles.ToList()
        };
    }

    /// <summary>
    /// Devuelve el listado de roles leido desde un fichero json.
    /// </summary>
    /// <returns></returns>
    [HttpGet("roles")]
    public async Task<IActionResult> Roles()
    {
        var f = await System.IO.File.ReadAllTextAsync("Data/roles.json");

        // var r = new JsonTextReader(f);
        // var json = JsonConvert.DeserializeObject(f);
        // return new JsonResult(json);
        return new JsonResult("");
    }

    /// <summary>
    /// Crea un nuevo usuario
    /// </summary>
    /// <param name="register">Objeto que contiene {nombres: string, apellidos:string, usuario:string, contraseña: string, confirmacion-contraseña:string}</param>
    /// <returns></returns>
    [HttpPost("add-user")]
    public async Task<IActionResult> AddUser([FromBody] Register register)
    {
        if (ModelState.IsValid)
        {
            var user = new Usuario() { Email = register.Email, UserName = register.Username, Nombres = register.Name, Apellidos = register.LastName, Activo = true };
            var result = await _userManager.CreateAsync(user, register.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("Usuario {0} creado correctamente por el usuario {1}.", register.Username, User!.Identity!.Name);
                return Ok(new AccountDto
                {
                    UserId = user.Id,
                    Name = user.Nombres,
                    LastName = user.Apellidos,
                    Username = user.UserName,
                    Email = user.Email,
                    Active = user.Activo,
                    Roles = new List<string>()
                });
            }
            return BadRequest(result);
        }
        return BadRequest(new { Result = false, Message = "Error agregando el usuario." });
    }

    /// <summary>
    /// Edita el nombre y los apellidos de un usuario
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    [HttpPost("edit-user")]
    public async Task<IActionResult> EditUser([FromBody] EditUserNameDto user)
    {
        if (ModelState.IsValid)
        {
            var userInDb = await _db.Set<Usuario>().FindAsync(user.Id);
            if (userInDb == null)
            {
                return BadRequest("No existe el usuario solicitado");
            }
            userInDb.Nombres = user.Name;
            userInDb.Apellidos = user.LastName;
            var result = await _db.SaveChangesAsync();
            if (result == 1)
            {
                _logger.LogInformation("Usuario {0} modificado correctamente por el usuario {1}.", userInDb.UserName, User!.Identity!.Name);
                return Ok();
            }
        }
        return BadRequest(new { Result = false, Message = "Error modificando el usuario." });
    }

    /// <summary>
    /// Resetea la contraseña de un usuario
    /// </summary>
    /// <param name="resetPassword"></param>
    /// <returns></returns>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPassword resetPassword)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(resetPassword.UsuarioId);
            if (user == null)
            {
                return BadRequest("No existe el usuario solicitado");
            }
            await _userManager.RemovePasswordAsync(user);
            var result = await _userManager.AddPasswordAsync(user, resetPassword.Contraseña);
            if (result.Succeeded)
            {
                _logger.LogInformation($"Se reseteo la contraseña del usuario {user.UserName} por el usuario {User!.Identity!.Name}.");
                return Ok();
            }
            return BadRequest(result.Errors);
        }
        return BadRequest(new { Result = false, Message = "Error resetando la contraseña." });
    }

    /// <summary>
    /// Cambiar la contraseña por el usuario
    /// </summary>
    /// <param name="changePassword"></param>
    /// <returns></returns>
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePassword changePassword)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(changePassword.UsuarioId);
            if (user == null)
            {
                return BadRequest("No existe el usuario solicitado");
            }
            var result = await _userManager.ChangePasswordAsync(user, changePassword.ContraseñaActual, changePassword.Contraseña);

            if (result.Succeeded)
            {
                _logger.LogInformation($"Se cambio la contraseña del usuario {user.UserName} correctamente.");
                return Ok();
            }
            return BadRequest(result.Errors);
        }
        return BadRequest(new { Result = false, Message = "Error cambiando la contraseña." });
    }

    /// <summary>
    /// Cambia el estado de un usuario en activo o no
    /// </summary>
    /// <param name="idUsuario">Id del usuario al que se le va a cambiar el estado</param>
    /// <returns></returns>
    [HttpGet("cambiar-estado")]
    public async Task<IActionResult> CambiarEstadoUsuario(string idUsuario)
    {
        var usuario = await _db.Set<Usuario>().FindAsync(idUsuario);
        if (usuario == null)
        {
            return NotFound();
        }
        usuario.Activo = !usuario.Activo;
        await _db.SaveChangesAsync();
        _logger.LogInformation($"Se cambio el estado del usuario {usuario.UserName} a {usuario.Activo} por el usuario {User!.Identity!.Name}.");
        return Ok();
    }

    /// <summary>
    /// Reemplaza el listado de roles de un usuario por uno nuevo
    /// </summary>
    /// <param name="rolesDto"></param>
    /// <returns>Un objeto con {Resultado: bool, Mensaje:string}</returns>
    [HttpPost]
    [Route("cambiar-roles")]
    public async Task<IActionResult> CambiarRoles([FromBody] CambiarRolesDto rolesDto)
    {
        var usuario = await _userManager.FindByIdAsync(rolesDto.idUsuario);
        if (usuario == null)
        {
            return NotFound();
        }
        var rolesActuales = await _userManager.GetRolesAsync(usuario);
        var result = await _userManager.RemoveFromRolesAsync(usuario, rolesActuales);
        if (!result.Succeeded)
        {
            return BadRequest(new { Resultado = false, Mensaje = "Ocurrieron errores modificando los roles: " + String.Join(',', result.Errors.Select(e => e.Description)) });
        }
        foreach (var rol in rolesDto.Roles)
        {
            if (!_db.Set<IdentityRole>().Any(r => r.Name == rol))
            {
                _db.Add(new IdentityRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = rol,
                    NormalizedName = rol.ToUpper()
                });
                await _db.SaveChangesAsync();
            }
        }

        result = await _userManager.AddToRolesAsync(usuario, rolesDto.Roles);
        if (!result.Succeeded)
        {
            return BadRequest(new { Resultado = false, Mensaje = "Ocurrieron errores modificando los roles" + String.Join(',', result.Errors.Select(e => e.Description)) });
        }
        _logger.LogInformation($"Se cambiaron correctamente los roles del usuario {usuario.UserName}, los roles nuevos son {String.Join(',', rolesDto.Roles)}. Cambio realizado por el usuario {User!.Identity!.Name}.");
        return Ok(new { Resultado = true, Mensaje = "Roles modificados correctamente." });
    }

    /// <summary>
    /// Autentica un usuario
    /// </summary>
    /// <param name="login">Objeto que contiene {usuario:string, contraseña:string}</param>
    /// <returns>Si es correcta la autenticacion retorna un JWT, sino un mensaje de error</returns>
    [HttpPost]
    [Route("Login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] Login login)
    {
        if (ModelState.IsValid)
        {
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(login.UserName, login.Password, true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(login.UserName);
                var roles = await _userManager.GetRolesAsync(user);
                var expiration = DateTime.UtcNow.AddDays(1);
                var userId = "ee41c414-ae4f-4f68-b905-c11606a2a226";
                var token = BuildToken(login.UserName, user.Email, expiration, roles);
                return Ok(new
                {
                    token = token,
                    expiration = expiration,
                    userId = user.Id
                });
            }
            else
            {
                return BadRequest(new { Rsult = false, Message = "Intento de autenticacion incorrecto." });
            }
        }
        // If we got this far, something failed, redisplay form
        return BadRequest(new { Result = false, Message = "Error." });
    }

    #region Helpers

    /// <summary>
    /// Contruye un JWT para la autenticacion
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="email"></param>
    /// <param name="expiration"></param>
    /// <param name="roles"></param>
    /// <returns>Un JWT en forma de string</returns>
    private string BuildToken(string userName, string email, DateTime expiration, IList<string> roles)
    {
        var claims = new List<Claim> {
                new Claim (JwtRegisteredClaimNames.UniqueName, userName),
                new Claim (JwtRegisteredClaimNames.Jti, Guid.NewGuid ().ToString ()),
                new Claim (JwtRegisteredClaimNames.Email, email),

            };
        foreach (var rol in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, rol));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Authorization:Password"]));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            issuer: _config["Authorization:Issuer"],
            audience: _config["Authorization:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: cred
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    #endregion
}
