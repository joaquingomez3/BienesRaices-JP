using bienesraices.Models;
using bienesraices.Repositorios;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
namespace bienesraices.Controllers;

[Authorize] // Requiere que el usuario esté autenticado para acceder a cualquier acción en este controlador
public class PropietarioController : Controller
{
    private readonly ILogger<PropietarioController> _logger;
    private RepositorioPropietario repoPropietario;
    public PropietarioController(ILogger<PropietarioController> logger, IConfiguration configuration)
    {
        _logger = logger;
        repoPropietario = new RepositorioPropietario(configuration);
    }

    // [Authorize(Roles = "Admin")]
    public IActionResult Index()
    {
        var lista = repoPropietario.ObtenerPropietarios();
        return View(lista);
    }

    // crear propietario
    [HttpGet]
    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(Propietario propietario)
    {
        if (ModelState.IsValid)
        {
            repoPropietario.CrearPropietario(propietario);
            TempData["MensajeExito"] = "Propietario creado con éxito ✅";
            return RedirectToAction(nameof(Index));
        }
        return View(propietario);
    }
    //editar propietario
    [HttpGet]
    public IActionResult Editar(int id)
    {
        var propietario = repoPropietario.ObtenerPropietarioPorId(id);

        if (propietario == null)
        {
            return NotFound();
        }
        return View(propietario);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Editar(Propietario propietario)
    {
        if (ModelState.IsValid)
        {
            repoPropietario.ActualizarPropietario(propietario);
            TempData["MensajeExito"] = "Propietario editado con éxito ✅";
            return RedirectToAction(nameof(Index));
        }
        return View(propietario);
    }
    [HttpGet]
    public IActionResult Eliminar()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Eliminar(int id)
    {
        var propietario = new Propietario { Id = id };
        repoPropietario.EliminarPropietario(propietario);
        TempData["MensajeExito"] = "Propietario eliminado con éxito ✅";
        return RedirectToAction(nameof(Index));
    }


}