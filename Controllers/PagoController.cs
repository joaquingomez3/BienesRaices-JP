using Microsoft.AspNetCore.Mvc;
using bienesraices.Models;
using bienesraices.Repositorios;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[Authorize] // Requiere que el usuario esté autenticado para acceder a cualquier acción en este controlador

public class PagoController : Controller
{
    private readonly RepositorioPago repoPago;

    private readonly RepositorioContrato repoContrato;
    public PagoController(IConfiguration configuration)
    {
        repoPago = new RepositorioPago(configuration);
        repoContrato = new RepositorioContrato(configuration);
    }

    [HttpGet]
    public async Task<IActionResult> RegistroPagos(int contratoId, int page, int pageSize = 5)
    {

        page = page < 1 ? 1 : page;
        var totalPagos = await repoPago.ContarPagosPorActivos(contratoId);
        var pagos = await repoPago.PagosPaginados(contratoId, page, pageSize);

        ViewBag.TotalPages = (int)Math.Ceiling((double)totalPagos / pageSize);
        ViewBag.CurrentPage = page;

        ViewBag.ContratoId = contratoId;

        return View(pagos);
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> ListarContratos(string? dni, int page, int pageSize = 5)
    {
        dni = string.IsNullOrWhiteSpace(dni) ? null : $"%{dni}%";
        page = page < 1 ? 1 : page;
        var totalContratos = await repoContrato.ContarContratos(dni);
        var contratos = await repoContrato.ContratosPaginados(dni, page, pageSize);

        ViewBag.TotalPages = (int)Math.Ceiling((double)totalContratos / pageSize);
        ViewBag.CurrentPage = page;

        return View(contratos);
    }

    [HttpGet]
    public IActionResult Crear(int contratoId)
    {
        var contrato = repoContrato.ObtenerContratoPorId(contratoId);

        if (contrato == null)
            return NotFound();

        ViewBag.IdContrato = contratoId;
        ViewBag.MontoContrato = contrato.Monto_mensual; // 👈 acá mandamos el importe

        return View(new Pago
        {
            Id_contrato = contratoId,
            Importe = contrato.Monto_mensual, // 👈 también lo ponemos en el modelo
            Fecha_pago = DateTime.Today
        });
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(Pago pago)
    {

        var idUsuarioStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(idUsuarioStr, out int idUsuario))
        {
            pago.Id_usuario_creador = idUsuario;
        }

        if (ModelState.IsValid)
        {
            repoPago.Crear(pago);
            TempData["MensajeExito"] = "Pago registrado correctamente.";
            return RedirectToAction("RegistroPagos", new { contratoId = pago.Id_contrato });
        }

        ViewBag.IdContrato = pago.Id_contrato;
        ViewBag.MontoContrato = pago.Importe;
        return View(pago);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Eliminar(int id)
    {
        repoPago.Eliminar(id);
        TempData["MensajeExito"] = "Pago eliminado correctamente.";
        return RedirectToAction("RegistroPagos");
    }

}