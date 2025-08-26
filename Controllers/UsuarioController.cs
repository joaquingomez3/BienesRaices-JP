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
    public IActionResult Index()
    {
        var lista = repoUsuario.ObtenerUsuarios();
        return View(lista);
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
                new Claim(ClaimTypes.Role, usuarioEncontrado.RolUsuario.ToLower()) // Rol en minúsculas
            };
                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");

            }

        }
        else
        {
            ViewBag.Error = "Email o clave incorrectos";

        }
        return View();
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

    [HttpPost]
    public IActionResult CrearUsuario(Usuario usuario)
    {
        var hasher = new PasswordHasher<Usuario>();
        usuario.Password = hasher.HashPassword(usuario, usuario.Password);
        usuario.Activo = true;

        repoUsuario.Alta(usuario); // Método que guarda el usuario en la base
        return RedirectToAction("Index");
    }
}