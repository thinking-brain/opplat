using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Account.WebApi.Data;
using Account.WebApi.Dtos;
using Account.WebApi.Models;
using LdapForNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using static LdapForNet.Native.Native;

namespace Account.WebApi.Controllers {
    [Route ("auth/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private ILogger _logger { get; set; }
        private readonly DbContext _db;

        public AccountController (
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            ILogger<AccountController> logger, AccountDbContext context) {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _db = context;
        }

        /// <summary>
        /// Devuelve el listado de usuarios
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route ("usuarios")]
        public async Task<IActionResult> Usuarios () {
            var result = await _db.Set<Usuario> ().Select (u => new AccountDto {
                UserId = u.Id,
                    Nombres = u.Nombres,
                    Apellidos = u.Apellidos,
                    Username = u.UserName,
                    Activo = u.Activo,
                    Roles = (_userManager.GetRolesAsync (u)).Result.ToList ()
            }).ToListAsync ();
            return new JsonResult (result);
        }

        [HttpPost]
        [Route ("importarusuarios")]
        public async Task<IActionResult> ImportarUsuarios ([FromBody] LdapFormViewModel model) {
            int usuarios_importados = 0;

            if (ModelState.IsValid) {
                LdapConnection cn = new LdapConnection ();
                cn.Connect (model.Host, 389);

                cn.Timeout = new TimeSpan (0, 0, 10); // seconds
                try {
                    cn.Bind (LdapAuthType.Digest, new LdapCredential {
                        UserName = model.UserName,
                            Password = model.Password,
                    });
                } catch (Exception e) {
                    return Ok (new { status = 400, mensaje = e.Message });
                }
                string[] attr = { "cn", "sAMAccountName" };
                string @base = cn.GetRootDse ().Attributes["rootDomainNamingContext"][0].ToString ();
                if (model.UnidadOrganizativa.Length > 1) {
                    @base = "OU=" + model.UnidadOrganizativa.ToLower () + "," + @base;
                }
                try {
                    var entries = cn.Search (@base, "(objectClass=user)", attr, LdapSearchScope.LDAP_SCOPE_SUB);
                    foreach (var entry in entries) {
                        var full_domain_name = entry.Attributes["cn"][0].Split (" ");
                        var userName = entry.Attributes["sAMAccountName"][0];
                        var nombre = full_domain_name[0];
                        string apellidos = "null";
                        if (full_domain_name.Count () > 1) {
                            apellidos = full_domain_name[1];
                        };
                        var password = "Aa123456-";
                        var user = new Usuario { Nombres = nombre, Apellidos = apellidos, UserName = userName };
                        try {
                            var result = await _userManager.CreateAsync (user, password);
                            if (result.Succeeded) {
                                usuarios_importados += 1;
                            }
                        } catch (Exception e) {
                            var error = e.Message;
                        }
                    }
                } catch (Exception e) {
                    var error = "Ha ocurrido un error";
                    //-2146233088
                    if (e.HResult == -2146233088) {
                        error = "No existe la unidad organizativa";
                    }
                    return Ok (new { status = 400, mensaje = error });
                }
                cn.Dispose ();
            }
            string mensaje;
            if (usuarios_importados > 0) {
                mensaje = "Se han importado " + usuarios_importados + " usuarios";
            } else {
                mensaje = "No se encontraron usuarios nuevos para importar";
            }
            return Ok (new { status = 200, mensaje = mensaje });
        }

        /// <summary>
        /// Devuelve los datos del usuario
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route ("perfil")]
        public async Task<IActionResult> Perfil (string usuario) {
            var user = await _userManager.FindByNameAsync (usuario);
            if (user == null) {
                return BadRequest ("No existe el usuario solicitado");
            }
            var roles = await _userManager.GetRolesAsync (user);
            return Ok (new AccountDto {
                UserId = user.Id,
                    Nombres = user.Nombres,
                    Apellidos = user.Apellidos,
                    Username = user.UserName,
                    Activo = user.Activo,
                    Roles = roles.ToList ()
            });
        }

        /// <summary>
        /// Devuelve el listado de roles leido desde un fichero json.
        /// </summary>
        /// <returns></returns>
        [HttpGet ("roles")]
        public async Task<IActionResult> Roles () {
            var f = System.IO.File.ReadAllText ("Data/roles.json");

            // var r = new JsonTextReader(f);
            var json = JsonConvert.DeserializeObject (f);
            return new JsonResult (json);
        }

        /// <summary>
        /// Crea un nuevo usuario
        /// </summary>
        /// <param name="register">Objeto que contiene {nombres: string, apellidos:string, usuario:string, contraseña: string, confirmacion-contraseña:string}</param>
        /// <returns></returns>
        [HttpPost ("add-usuario")]
        public async Task<IActionResult> AddUsuario ([FromBody] Register register) {
            if (ModelState.IsValid) {
                var user = new Usuario () { Email = register.Username + "@nauta.cu", UserName = register.Username, Nombres = register.Nombres, Apellidos = register.Apellidos, Activo = true };
                var result = await _userManager.CreateAsync (user, register.Contraseña);
                if (result.Succeeded) {
                    _logger.LogInformation ($"Usuario {register.Username} creado correctamente por el usuario {User.Identity.Name}.");
                    return Ok (new AccountDto {
                        UserId = user.Id,
                            Nombres = user.Nombres,
                            Apellidos = user.Apellidos,
                            Username = user.UserName,
                            Activo = user.Activo,
                            Roles = new List<string> ()
                    });
                }
                return BadRequest (result);
            }
            return BadRequest (new { Result = false, Message = "Error agregando el usuario." });
        }

        /// <summary>
        /// Edita el nombre y los apellidos de un usuario
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        [HttpPost ("editar-usuario")]
        public async Task<IActionResult> EditarUsuario ([FromBody] EditarUsuario usuario) {
            if (ModelState.IsValid) {
                var user = await _db.Set<Usuario> ().FindAsync (usuario.Id);
                if (user == null) {
                    return BadRequest ("No existe el usuario solicitado");
                }
                user.Nombres = usuario.Nombres;
                user.Apellidos = usuario.Apellidos;
                var result = await _db.SaveChangesAsync ();
                _logger.LogInformation ($"Usuario {user.UserName} modificado correctamente por el usuario {User.Identity.Name}.");
                return Ok ();
            }
            return BadRequest (new { Result = false, Message = "Error modificando el usuario." });
        }

        /// <summary>
        /// Resetea la contraseña de un usuario
        /// </summary>
        /// <param name="resetPassword"></param>
        /// <returns></returns>
        [HttpPost ("reset-password")]
        public async Task<IActionResult> ResetPassword ([FromBody] ResetPassword resetPassword) {
            if (ModelState.IsValid) {
                var user = await _userManager.FindByIdAsync (resetPassword.UsuarioId);
                if (user == null) {
                    return BadRequest ("No existe el usuario solicitado");
                }
                await _userManager.RemovePasswordAsync (user);
                var result = await _userManager.AddPasswordAsync (user, resetPassword.Contraseña);
                if (result.Succeeded) {
                    _logger.LogInformation ($"Se reseteo la contraseña del usuario {user.UserName} por el usuario {User.Identity.Name}.");
                    return Ok ();
                }
                return BadRequest (result.Errors);
            }
            return BadRequest (new { Result = false, Message = "Error resetando la contraseña." });
        }

        /// <summary>
        /// Cambiar la contraseña por el usuario
        /// </summary>
        /// <param name="changePassword"></param>
        /// <returns></returns>
        [HttpPost ("change-password")]
        public async Task<IActionResult> ChangePassword ([FromBody] ChangePassword changePassword) {
            if (ModelState.IsValid) {
                var user = await _userManager.FindByIdAsync (changePassword.UsuarioId);
                if (user == null) {
                    return BadRequest ("No existe el usuario solicitado");
                }
                var result = await _userManager.ChangePasswordAsync (user, changePassword.ContraseñaActual, changePassword.Contraseña);

                if (result.Succeeded) {
                    _logger.LogInformation ($"Se cambio la contraseña del usuario {user.UserName} correctamente.");
                    return Ok ();
                }
                return BadRequest (result.Errors);
            }
            return BadRequest (new { Result = false, Message = "Error cambiando la contraseña." });
        }

        /// <summary>
        /// Cambia el estado de un usuario en activo o no
        /// </summary>
        /// <param name="idUsuario">Id del usuario al que se le va a cambiar el estado</param>
        /// <returns></returns>
        [HttpGet ("cambiar-estado")]
        public async Task<IActionResult> CambiarEstadoUsuario (string idUsuario) {
            var usuario = await _db.Set<Usuario> ().FindAsync (idUsuario);
            if (usuario == null) {
                return NotFound ();
            }
            usuario.Activo = !usuario.Activo;
            await _db.SaveChangesAsync ();
            _logger.LogInformation ($"Se cambio el estado del usuario {usuario.UserName} a {usuario.Activo} por el usuario {User.Identity.Name}.");
            return Ok ();
        }

        /// <summary>
        /// Reemplaza el listado de roles de un usuario por uno nuevo
        /// </summary>
        /// <param name="idUsuario">Id del usuario al que se le van a cambiar los roles</param>
        /// <param name="roles">Listado de nuevos roles</param>
        /// <returns>Un objeto con {Resultado: bool, Mensaje:string}</returns>
        [HttpPost]
        [Route ("cambiar-roles")]
        public async Task<IActionResult> CambiarRoles ([FromBody] CambiarRolesDto rolesDto) {
            var usuario = await _userManager.FindByIdAsync (rolesDto.idUsuario);
            if (usuario == null) {
                return NotFound ();
            }
            var rolesActuales = await _userManager.GetRolesAsync (usuario);
            var result = await _userManager.RemoveFromRolesAsync (usuario, rolesActuales);
            if (!result.Succeeded) {
                return BadRequest (new { Resultado = false, Mensaje = "Ocurrieron errores modificando los roles: " + String.Join (',', result.Errors.Select (e => e.Description)) });
            }
            foreach (var rol in rolesDto.Roles) {
                if (!_db.Set<IdentityRole> ().Any (r => r.Name == rol)) {
                    _db.Add (new IdentityRole {
                        Id = Guid.NewGuid ().ToString (),
                            Name = rol,
                            NormalizedName = rol.ToUpper ()
                    });
                    await _db.SaveChangesAsync ();
                }
            }

            result = await _userManager.AddToRolesAsync (usuario, rolesDto.Roles);
            if (!result.Succeeded) {
                return BadRequest (new { Resultado = false, Mensaje = "Ocurrieron errores modificando los roles" + String.Join (',', result.Errors.Select (e => e.Description)) });
            }
            _logger.LogInformation ($"Se cambiaron correctamente los roles del usuario {usuario.UserName}, los roles nuevos son {String.Join(',', rolesDto.Roles)}. Cambio realizado por el usuario {User.Identity.Name}.");
            return Ok (new { Resultado = true, Mensaje = "Roles modificados correctamente." });
        }

        /// <summary>
        /// Autentica un usuario
        /// </summary>
        /// <param name="login">Objeto que contiene {usuario:string, contraseña:string}</param>
        /// <returns>Si es correcta la autenticacion retorna un JWT, sino un mensaje de error</returns>
        [HttpPost]
        [Route ("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login ([FromBody] Login login) {
            if (ModelState.IsValid) {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync (login.UserName, login.Password, true, lockoutOnFailure : false);
                if (result.Succeeded) {
                    var user = await _userManager.FindByNameAsync (login.UserName);
                    var roles = await _userManager.GetRolesAsync (user);
                    var expiration = DateTime.UtcNow.AddDays (1);
                    var userId="ee41c414-ae4f-4f68-b905-c11606a2a226";
                    var token = BuildToken (login.UserName, login.UserName + "@microapp.cu", expiration, roles);
                    return Ok (new {
                        token = token,
                            expiration = expiration,
                            userId=userId
                    });
                } else {
                    return BadRequest (new { Rsult = false, Message = "Intento de autenticacion incorrecto." });
                }
            }
            // If we got this far, something failed, redisplay form
            return BadRequest (new { Result = false, Message = "Error." });
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
        private string BuildToken (string userName, string email, DateTime expiration, IList<string> roles) {
            var claims = new List<Claim> {
                new Claim (JwtRegisteredClaimNames.UniqueName, userName),
                new Claim (JwtRegisteredClaimNames.Jti, Guid.NewGuid ().ToString ()),
                new Claim (JwtRegisteredClaimNames.Email, email),

            };
            foreach (var rol in roles) {
                claims.Add (new Claim (ClaimTypes.Role, rol));
            }

            var key = new SymmetricSecurityKey (Encoding.UTF8.GetBytes ("Admin123*1234567890"));
            var cred = new SigningCredentials (key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken (
                issuer: "microapp.cu",
                audience: "microapp.cu",
                claims : claims,
                expires : expiration,
                signingCredentials : cred
            );
            return new JwtSecurityTokenHandler ().WriteToken (token);
        }

        #endregion     
    }
}