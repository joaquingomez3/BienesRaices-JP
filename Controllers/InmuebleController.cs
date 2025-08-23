using bienesraices.Models;
using bienesraices.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
namespace bienesraices.Controllers;

[Authorize] // Requiere que el usuario esté autenticado para acceder a cualquier acción en este controlador
public class InmuebleController : Controller
{
    private readonly RepositorioInmueble repoInmueble;
    private readonly RepositorioPropietario repoPropietario;
    private readonly RepositorioTipoInmueble repoTipo;

    public InmuebleController(IConfiguration configuration)
    {
        repoInmueble = new RepositorioInmueble(configuration);
        repoPropietario = new RepositorioPropietario(configuration);
        repoTipo = new RepositorioTipoInmueble(configuration);
    }

    public IActionResult Index()
    {
        var lista = repoInmueble.ObtenerInmuebles();
        return View(lista);
    }

    [HttpGet]
    public IActionResult Crear()
    {
        ViewBag.Usos = new SelectList(new[] { "Comercial", "Residencial" });
        ViewBag.Estados = new SelectList(new[] { "Disponible", "Ocupado", "Reservado" });
        ViewBag.Propietarios = repoPropietario.ObtenerPropietarios();
        ViewBag.Tipos = repoTipo.ObtenerTiposInmueble();
        return View();
    }

    [HttpPost]
    public IActionResult Crear(Inmueble inmueble)
    {
        if (ModelState.IsValid)
        {
            repoInmueble.CrearInmueble(inmueble);
            return RedirectToAction("Index");
        }

        // Si falla, volvemos a cargar los combos
        ViewBag.Usos = new SelectList(new[] { "Comercial", "Residencial" });
        ViewBag.Estados = new SelectList(new[] { "Disponible", "Ocupado", "Reservado" });
        ViewBag.Propietarios = repoPropietario.ObtenerPropietarios();
        ViewBag.Tipos = repoTipo.ObtenerTiposInmueble();
        return View(inmueble);
    }
}
