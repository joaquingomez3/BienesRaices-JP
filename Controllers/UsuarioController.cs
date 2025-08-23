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
        var usuarioEncontrado = repoUsuario.ObtenerUsuarioPorEmailClave(usuario.Email, usuario.Password);
        if (usuarioEncontrado != null)
        {
            var claims = new List<Claim>
            {

                new Claim(ClaimTypes.Name, usuarioEncontrado.Email),
                new Claim(ClaimTypes.Role, usuarioEncontrado.RolUsuario.ToLower()) // Rol en minúsculas
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home"); // redirige al Home
        }
        else
        {
            ViewBag.Error = "Email o clave incorrectos";
            return View();
        }
    }


    public async Task<IActionResult> Salir()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Login", "Usuario");
    }
}