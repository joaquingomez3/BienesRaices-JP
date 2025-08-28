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

    // public IActionResult Index()
    // {
    //     var lista = repoInmueble.ObtenerInmuebles();
    //     return View(lista);
    // }

    [HttpGet]
    public async Task<IActionResult> Index(int page, int pageSize = 5)
    {
        page = page < 1 ? 1 : page;
        var totalInmuebles = await repoInmueble.ContarInmuebles();
        var inmuebles = await repoInmueble.InmueblesPaginados(page, pageSize);

        ViewBag.TotalPages = (int)Math.Ceiling((double)totalInmuebles / pageSize);
        ViewBag.CurrentPage = page;

        return View(inmuebles);
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
    [HttpGet]
    public IActionResult Editar(int id)
    {
        var inmueble = repoInmueble.ObtenerPorId(id);
        if (inmueble == null)
        {
            return NotFound();
        }

        var propietarios = repoPropietario.ObtenerPropietarios()
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Dni} - {p.Apellido} {p.Nombre}"
            }).ToList();

        ViewBag.Propietarios = propietarios;
        ViewBag.Tipos = repoTipo.ObtenerTiposInmueble();

        return View(inmueble);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Editar(int id, Inmueble inmueble)
    {

        if (id != inmueble.Id) return BadRequest();

        if (ModelState.IsValid)
        {
            repoInmueble.Editar(inmueble);
            TempData["MensajeExito"] = "Inmueble actualizado correctamente";
            return RedirectToAction("Index");
        }

        var propietarios = repoPropietario.ObtenerPropietarios()
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Dni} - {p.Apellido} {p.Nombre}"
            }).ToList();

        ViewBag.Propietarios = propietarios;

        ViewBag.Tipos = repoTipo.ObtenerTiposInmueble();
        return View(inmueble);
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
        var inmueble = new Inmueble { Id = id };
        repoInmueble.EliminarInmueble(inmueble);
        TempData["MensajeExito"] = "Inmueble eliminado con éxito ✅";
        return RedirectToAction(nameof(Index));
    }
    
    
}
