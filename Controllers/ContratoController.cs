using bienesraices.Repositorios;
using bienesraices.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Forms;


public class ContratoController : Controller
{
    private readonly RepositorioContrato repoContrato;
    private readonly RepositorioInquilino repoInquilino;
    private readonly RepositorioInmueble repoInmueble;

    public ContratoController(IConfiguration configuration)
    {
        repoContrato = new RepositorioContrato(configuration);
        repoInquilino = new RepositorioInquilino(configuration);
        repoInmueble = new RepositorioInmueble(configuration);
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? dni, int page, int pageSize = 5)
    {
        dni = string.IsNullOrWhiteSpace(dni) ? null : $"%{dni}%";
        page = page < 1 ? 1 : page;
        var totalContratos = await repoContrato.ContarContratos(dni);
        var contratos = await repoContrato.ContratosPaginados(dni, page, pageSize);

        ViewBag.TotalPages = (int)Math.Ceiling((double)totalContratos / pageSize);
        ViewBag.CurrentPage = page;

        return View(contratos);
    }

    [HttpGet("/Index.cshtml")]
    public IActionResult Detalle(int id)
    {
        var contrato = repoContrato.ObtenerContratoPorId(id);
        if (contrato == null)
        {
            return NotFound();
        }
        return View(contrato);
    }

    [HttpGet]
    public IActionResult Crear()
    {
        ViewBag.Inquilinos = new SelectList(repoInquilino.ObtenerInquilinosActivos(), "Id", "Nombre_completo");
        ViewBag.Inmuebles = new SelectList(repoInmueble.ObtenerInmueblesDisponibles(), "Id", "Direccion");


        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(Contrato contrato)
    {
        var idUsuarioStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(idUsuarioStr, out int idUsuario))
        {
            contrato.Id_usuario_creador = idUsuario;
        }


        var inmueble = repoInmueble.ObtenerPorId(contrato.Id_inmueble);
        if (inmueble != null)
        {
            contrato.Monto_mensual = inmueble.Precio;
        }

        if (ModelState.IsValid)
        {
            repoContrato.CrearContrato(contrato);
            return RedirectToAction("Index");
        }

        ViewBag.Inquilinos = new SelectList(repoInquilino.ObtenerInquilinosActivos(), "Id", "Nombre_completo");
        ViewBag.Inmuebles = new SelectList(repoInmueble.ObtenerInmueblesDisponibles(), "Id", "Direccion");

        return View(contrato);
    }
    [HttpGet]
    public IActionResult ObtenerPrecioInmueble(int id)
    {
        var inmueble = repoInmueble.ObtenerPorId(id);
        if (inmueble == null)
        {
            return NotFound();
        }
        
        return Ok(new { precio = inmueble.Precio });
    }

}