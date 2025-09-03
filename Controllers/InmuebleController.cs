using bienesraices.Models;
using bienesraices.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System;
namespace bienesraices.Controllers;

[Authorize] // Requiere que el usuario esté autenticado para acceder a cualquier acción en este controlador
public class InmuebleController : Controller
{
    private readonly RepositorioInmueble repoInmueble;
    private readonly RepositorioPropietario repoPropietario;
    private readonly RepositorioTipoInmueble repoTipo;

    private readonly RepositorioFotoInmueble repoFoto;

    public InmuebleController(IConfiguration configuration)
    {
        repoInmueble = new RepositorioInmueble(configuration);
        repoPropietario = new RepositorioPropietario(configuration);
        repoTipo = new RepositorioTipoInmueble(configuration);
        repoFoto = new RepositorioFotoInmueble(configuration);
    }

    // public IActionResult Index()
    // {
    //     var lista = repoInmueble.ObtenerInmuebles();
    //     return View(lista);
    // }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 5, string? propietario = null, string? estado = null)
    {
        page = page < 1 ? 1 : page;

        // 🔹 traer datos paginados
        var inmuebles = await repoInmueble.InmueblesFiltrados(page, pageSize, propietario, estado);

        // 🔹 contar total con filtros
        var totalInmuebles = await repoInmueble.ContarInmuebles(propietario, estado);

        foreach (var inmueble in inmuebles)
        {
            inmueble.Fotos = repoFoto.ObtenerFotosPorInmuebleId(inmueble.Id);
        }

        ViewBag.TotalPages = (int)Math.Ceiling((double)totalInmuebles / pageSize);
        ViewBag.CurrentPage = page;
        ViewBag.FiltroPropietario = propietario;
        ViewBag.FiltroEstado = estado;

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
            TempData["MensajeExito"] = "Inmueble actualizado correctamente";
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
        TempData["idInmueble"] = id;
        if (inmueble == null)
        {
            return NotFound();
        }

        inmueble.Fotos = repoFoto.ObtenerFotosPorInmuebleId(id);

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
    [ActionName("Editar")]
    public IActionResult EditarPost(int id, Inmueble inmueble, List<IFormFile> fotosLocales, string? urlFoto)
    {
        if (id != inmueble.Id) return BadRequest();

        if (ModelState.IsValid)
        {
            // 1) Actualizar datos del inmueble
            repoInmueble.Editar(inmueble);

            // 2) Guardar fotos subidas (a BD en longblob)
            if (fotosLocales != null && fotosLocales.Any())
            {
                foreach (var file in fotosLocales)
                {
                    if (file != null && file.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        file.CopyTo(ms);
                        var bytes = ms.ToArray();

                        repoFoto.AgregarFoto(new FotoInmueble
                        {
                            Id_inmueble = inmueble.Id,
                            Archivo = bytes,   // 🔹 guardamos en longblob
                            Url = null         // 🔹 sin URL
                        });
                    }
                }
            }

            // 3) (opcional) si vino una URL, la guardamos como URL (sin blob)
            if (!string.IsNullOrWhiteSpace(urlFoto))
            {
                repoFoto.AgregarFoto(new FotoInmueble
                {
                    Id_inmueble = inmueble.Id,
                    Url = urlFoto,
                    Archivo = null
                });
            }

            TempData["MensajeExito"] = "Inmueble actualizado correctamente";
            return RedirectToAction("Index");
        }

        // si hay errores, recargamos combos y devolvemos la vista
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

    public IActionResult VerFoto(int id)
    {
        var foto = repoFoto.ObtenerFotoPorId(id);
        if (foto?.Archivo == null) return NotFound();
        return File(foto.Archivo, "image/jpeg");
    }

    [HttpPost]
    public IActionResult EliminarFoto(int id)
    {
        repoFoto.EliminarFoto(id);
        return RedirectToAction("Editar", new { id = TempData["IdInmueble"] });
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

    [HttpGet]
    public JsonResult BuscarPropietarios(string term)
    {
        var propietarios = repoPropietario.buscarPropietarios(term);

        return Json(propietarios);
    }
}
