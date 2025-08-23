using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using bienesraices.Models;
using bienesraices.Repositorios;
using Microsoft.AspNetCore.Authorization;
namespace bienesraices.Controllers;

[Authorize] // Requiere que el usuario esté autenticado para acceder a cualquier acción en este controlador
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        //ViewBag.Usuario = HttpContext.Session.GetString("UsuarioNombre") ?? "Usuario";
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
