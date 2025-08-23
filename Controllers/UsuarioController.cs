using bienesraices.Repositorios;
using bienesraices.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
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
    public IActionResult Login(string email, string password)
    {
        var usuario = repoUsuario.ObtenerUsuarioPorEmailClave(email, password);
            if (usuario != null)
            {
               

                return RedirectToAction("Index", "Home"); // redirige al Home
            }
            else
            {
                ViewBag.Error = "Email o clave incorrectos";
                return View();
            }
    }        
}