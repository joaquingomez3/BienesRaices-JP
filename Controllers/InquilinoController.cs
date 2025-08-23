using bienesraices.Models;
using bienesraices.Repositorios;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
namespace bienesraices.Controllers;

[Authorize] // Requiere que el usuario esté autenticado para acceder a cualquier acción en este controlador
public class InquilinoController : Controller
{
    private readonly ILogger<InquilinoController> _logger;
    private RepositorioInquilino repoInquilino;
    public InquilinoController(ILogger<InquilinoController> logger, IConfiguration configuration)
    {
        _logger = logger;
        repoInquilino = new RepositorioInquilino(configuration);
    }
    public IActionResult Index()
    {
        var lista = repoInquilino.ObtenerInquilinos();
        return View(lista);
    }

    // crear inquilino
    [HttpGet]
    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(Inquilino inquilino)
    {
        if (ModelState.IsValid)
        {
            repoInquilino.CrearInquilino(inquilino);
            TempData["MensajeExito"] = "Inquilino creado con éxito ✅";
            return RedirectToAction(nameof(Index));
        }
        return View(inquilino);
    }
    //editar inquilino
    [HttpGet]
    public IActionResult Editar(int id)
    {
        var inquilino = repoInquilino.ObtenerPropietarioPorId(id);

        if (inquilino == null)
        {
            return NotFound();
        }
        return View(inquilino);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Editar(Inquilino inquilino)
    {
        if (ModelState.IsValid)
        {
            repoInquilino.ActualizarInquilino(inquilino);
            TempData["MensajeExito"] = "Inquilino editado con éxito ✅";
            return RedirectToAction(nameof(Index));
        }
        return View(inquilino);
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
        var inquilino = new Inquilino { Id = id };
        repoInquilino.EliminarInquilino(inquilino);
        TempData["MensajeExito"] = "Inquilino eliminado con éxito ✅";
        return RedirectToAction(nameof(Index));
    }



}