using Microsoft.AspNetCore.Mvc;
using bienesraices.Repositorios;


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
    public async Task<IActionResult> RegistroPagos(int id,int page, int pageSize = 5)
    {
        page = page < 1 ? 1 : page;
        var totalPagos = await repoPago.ContarPagosPorActivos(id);
        var pagos = await repoPago.PagosPaginados(id,page, pageSize);

        ViewBag.TotalPages = (int)Math.Ceiling((double)totalPagos / pageSize);
        ViewBag.CurrentPage = page;
        ViewBag.ContratoId = id;

        return View(pagos);
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
   

}