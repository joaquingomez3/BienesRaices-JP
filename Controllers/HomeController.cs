using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using bienesraices.Models;
using bienesraices.Repositorios;
namespace bienesraices.Controllers;

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
