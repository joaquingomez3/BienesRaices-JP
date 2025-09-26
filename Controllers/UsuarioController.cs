using bienesraices.Repositorios;
using bienesraices.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
namespace bienesraices.Controllers;

public class UsuarioController : Controller
{
    private readonly ILogger<UsuarioController> _logger;
    private RepositorioUsuario repoUsuario;
    public UsuarioController(ILogger<UsuarioController> logger, IConfiguration configuration)
    {
        _logger = logger;
        repoUsuario = new RepositorioUsuario(configuration);
    }

    [Authorize] // Requiere que el usuario esté autenticado para acceder a cualquier acción en este controlador
    [HttpGet]
    public async Task<IActionResult> Index(int page, int pageSize = 5)
    {
        page = page < 1 ? 1 : page;
        var totalUsuarios = await repoUsuario.ContarUsuarios();
        var usuarios = await repoUsuario.UsuariosPaginados(page, pageSize);

        ViewBag.TotalPages = (int)Math.Ceiling((double)totalUsuarios / pageSize);
        ViewBag.CurrentPage = page;

        return View(usuarios);
    }


    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(Usuario usuario)
    {
        var usuarioEncontrado = repoUsuario.ObtenerUsuarioPorEmail(usuario.Email);
        if (usuarioEncontrado != null)
        {
            var hasher = new PasswordHasher<Usuario>();
            var res = hasher.VerifyHashedPassword(usuarioEncontrado, usuarioEncontrado.Password, usuario.Password);

            if (res == PasswordVerificationResult.Success)
            {
                var claims = new List<Claim>
            {

                new Claim(ClaimTypes.Name, usuarioEncontrado.Nombre_usuario+ " "+usuarioEncontrado.Apellido_usuario),
                new Claim(ClaimTypes.Role, usuarioEncontrado.RolUsuario.ToLower()), // Rol en minúsculas
                new Claim(ClaimTypes.NameIdentifier, usuarioEncontrado.Id.ToString()),
                new Claim("Foto", usuarioEncontrado.Foto ?? ""),
            };
                if (!string.IsNullOrEmpty(usuarioEncontrado.Foto))
                {
                    claims.Add(new Claim("FotoPerfil", usuarioEncontrado.Foto));
                }
                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");

            }
            else
            {
                ViewBag.Error = "Email o clave incorrectos";
            }

        }
        else
        {
            ViewBag.Error = "Email o clave incorrectos";

        }
        return View();
    }


    [Authorize] // Requiere que el usuario esté autenticado para acceder a cualquier acción en este controlador
    [HttpPost]
    public async Task<IActionResult> CrearUsuario(Usuario usuario, IFormFile Foto)
    {
        if (ModelState.IsValid)
        {
            if (Foto != null && Foto.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Foto.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Foto.CopyToAsync(stream);
                }

                usuario.Foto = "/uploads/" + fileName;
            }
            var hasher = new PasswordHasher<Usuario>();
            usuario.Password = hasher.HashPassword(usuario, usuario.Password);
            usuario.Activo = true;

            repoUsuario.Alta(usuario); // Método que guarda el usuario en la base
            TempData["MensajeExito"] = "Usuario creado con éxito ✅";


        }
        return RedirectToAction("Index");

    }


    public async Task<IActionResult> Salir()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Login", "Usuario");
    }

    [HttpGet]

    public IActionResult Crear()
    {
        return View();
    }

    [Authorize] // Requiere que el usuario esté autenticado para acceder a cualquier acción en este controlador
    [HttpGet]
    public IActionResult Editar(int id)
    {
        var usuario = repoUsuario.ObtenerUsuarioPorId(id);

        if (usuario == null)
        {
            return NotFound();
        }

        return View(usuario);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(Usuario usuario, IFormFile? Foto)
    {
        if (ModelState.IsValid)
        {
            var usuarioExistente = repoUsuario.ObtenerUsuarioPorId(usuario.Id);

            if (usuarioExistente != null)
            {
                // Actualizar campos que no son la contraseña
                usuarioExistente.Nombre_usuario = usuario.Nombre_usuario;
                usuarioExistente.Apellido_usuario = usuario.Apellido_usuario;
                usuarioExistente.Email = usuario.Email;
                usuarioExistente.Id_tipo_usuario = usuario.Id_tipo_usuario;
                usuarioExistente.Activo = usuario.Activo;

                // Si subió una nueva foto
                if (Foto != null && Foto.Length > 0)
                {
                    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploads))
                        Directory.CreateDirectory(uploads);

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Foto.FileName);
                    var filePath = Path.Combine(uploads, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Foto.CopyToAsync(stream);
                    }

                    // Opcional: borrar la foto anterior del servidor si existe
                    if (!string.IsNullOrEmpty(usuarioExistente.Foto))
                    {
                        var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", usuarioExistente.Foto.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }

                    usuarioExistente.Foto = "/uploads/" + fileName;
                }

                // Solo actualizar la contraseña si el usuario ingresó una nueva
                if (!string.IsNullOrWhiteSpace(usuario.Password))
                {
                    var hasher = new PasswordHasher<Usuario>();
                    usuarioExistente.Password = hasher.HashPassword(usuario, usuario.Password);
                }

                repoUsuario.ActualizarUsuario(usuarioExistente);
                // si el usuario editado es el logueado, actualizamos las claims de la cookie para que el layout muestre la nueva foto
                var currentId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(currentId) && currentId == usuarioExistente.Id.ToString())
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuarioExistente.Nombre_usuario + " " + usuarioExistente.Apellido_usuario),
                        new Claim(ClaimTypes.Role, usuarioExistente.RolUsuario.ToLower()),
                        new Claim(ClaimTypes.NameIdentifier, usuarioExistente.Id.ToString()),
                        new Claim("Foto", usuarioExistente.Foto ?? "")
                    };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                }
                TempData["MensajeExito"] = "Usuario editado con éxito ✅";
                if (usuarioExistente.RolUsuario?.ToLower() == "empleado")
                    return RedirectToAction("Index", "Home"); // acción Index del controlador Home
                else
                    return RedirectToAction(nameof(Index));   // acción Index de UsuarioController

            }
            else
            {
                return NotFound();
            }
        }

        return View(usuario);
    }

    [HttpPost]

    public IActionResult EliminarFoto(int id)
    {
        var usuario = repoUsuario.ObtenerUsuarioPorId(id);
        if (usuario != null && !string.IsNullOrEmpty(usuario.Foto))
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", usuario.Foto.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            usuario.Foto = null;
            repoUsuario.ActualizarUsuario(usuario);
            TempData["MensajeExito"] = "Foto eliminada con éxito ✅";
        }

        return RedirectToAction(nameof(Editar), new { id });
    }


    public IActionResult Eliminar()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Eliminar(int id)
    {
        var usuario = new Usuario { Id = id };
        repoUsuario.EliminarUsuario(usuario);
        TempData["MensajeExito"] = "Usuario eliminado con éxito ✅";
        return RedirectToAction(nameof(Index));
    }

}