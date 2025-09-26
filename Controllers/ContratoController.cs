using bienesraices.Repositorios;
using bienesraices.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Authorization;
[Authorize] // Requiere que el usuario esté autenticado para acceder a cualquier acción en este controlador
public class ContratoController : Controller
{
    private readonly RepositorioContrato repoContrato;
    private readonly RepositorioInquilino repoInquilino;
    private readonly RepositorioInmueble repoInmueble;
    private readonly RepositorioPago repoPago;

    public ContratoController(IConfiguration configuration)
    {
        repoContrato = new RepositorioContrato(configuration);
        repoInquilino = new RepositorioInquilino(configuration);
        repoInmueble = new RepositorioInmueble(configuration);
        repoPago = new RepositorioPago(configuration);
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? dni, int page, int pageSize = 5)
    {
        dni = string.IsNullOrWhiteSpace(dni) ? null : $"%{dni}%";
        page = page < 1 ? 1 : page;

        var totalContratos = await repoContrato.ContarContratos(dni);
        var contratos = await repoContrato.ContratosPaginados(dni, page, pageSize);

        // 🔄 Actualizar estado si la fecha de fin ya pasó
        foreach (var contrato in contratos)
        {
            if (contrato.Estado == "Vigente" && contrato.Fecha_fin < DateTime.Today)
            {
                await repoContrato.ActualizarEstadoAsync(contrato.Id, "Finalizado");
                contrato.Estado = "Finalizado"; // Reflejar el cambio en la vista
            }
        }

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
    public IActionResult Crear(int idInmueble)
    {
        ViewBag.Inquilinos = new SelectList(repoInquilino.ObtenerInquilinosActivos(), "Id", "Nombre_completo");
        var inmueble = repoInmueble.ObtenerPorId(idInmueble);

        if (inmueble == null)
        {
            return NotFound();
        }

        var contrato = new Contrato
        {
            Id_inmueble = inmueble.Id,      // ✅ Cargar acá
            Monto_mensual = inmueble.Precio,
            Estado = "Activo"
        };


        ViewBag.InmuebleNombre = inmueble.Direccion;
        ViewBag.MontoMensual = inmueble.Precio;
        return View(contrato);
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
        // Validar fechas antes de usar en la consulta
        if (contrato.Fecha_inicio == default || contrato.Fecha_fin == default)
        {
            ModelState.AddModelError("", "Las fechas no son válidas.");
        }
        else
        {
            if (repoContrato.ExisteContratoActivoEnRango(
                contrato.Id_inmueble,
                contrato.Fecha_inicio,
                contrato.Fecha_fin))
            {
                ModelState.AddModelError("", "El inmueble ya tiene un contrato vigente en el rango de fechas seleccionado.");
            }
        }

        if (repoContrato.ExisteContratoActivoEnRango(contrato.Id_inmueble, contrato.Fecha_inicio, contrato.Fecha_fin))
        {
            ModelState.AddModelError("", "El inmueble ya tiene un contrato vigente en el rango de fechas seleccionado.");
        }



        if (ModelState.IsValid)
        {
            repoContrato.CrearContrato(contrato);
            TempData["MensajeExito"] = "Contrato creado correctamente";
            return RedirectToAction("Index");
        }


        if (!ModelState.IsValid)
        {
            foreach (var entry in ModelState)
            {
                var key = entry.Key;
                var errors = entry.Value.Errors;

                foreach (var error in errors)
                {
                    Console.WriteLine($"Error en '{key}': {error.ErrorMessage}");
                }
            }
        }
        ViewBag.Inquilinos = new SelectList(repoInquilino.ObtenerInquilinosActivos(), "Id", "Nombre_completo");
        ViewBag.Inmuebles = new SelectList(repoInmueble.ObtenerInmueblesDisponibles(), "Id", "Direccion");
        Console.WriteLine("algo salio mal");

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
    [HttpPost]
    public IActionResult Rescindir(int Id, DateTime FechaRescision, decimal Multa)
    {
        var contrato = repoContrato.ObtenerContratoPorId(Id);


        if (contrato == null) return NotFound();



        var idUsuarioStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(idUsuarioStr, out int idUsuario))
        {
            contrato.Id_usuario_finalizador = idUsuario;
        }


        // Guardar cambios en BD (Estado, fecha de rescisión, multa, etc.)
        contrato.Estado = "Rescindido";
        contrato.Fecha_terminacion = FechaRescision;
        contrato.Multa = Multa;

        repoPago.Crear(new Pago
        {
            Id_contrato = contrato.Id,
            Numero_pago = 0,
            Fecha_pago = FechaRescision,
            Detalle = "Multa por rescisión anticipada",
            Importe = Multa,
            Estado = "PAGADO",
            Id_usuario_creador = contrato.Id_usuario_finalizador ?? 0,
        });
        repoContrato.ActualizarContrato(contrato);
        TempData["MensajeExito"] = "Se Rescindio Contrato correctamente";

        return RedirectToAction("RegistroPagos", "Pago", new { contratoId = contrato.Id });
    }

    [HttpPost]
    public async Task<IActionResult> CalcularMulta(int idContrato, DateTime fechaRescision)
    {
        var contrato = repoContrato.ObtenerContratoPorId(idContrato);
        if (contrato == null) return NotFound();

        DateTime fechaInicio = (DateTime)contrato.Fecha_inicio;
        DateTime fechaFinalizacionOriginal = (DateTime)contrato.Fecha_fin;
        var duracionTotal = fechaFinalizacionOriginal - fechaInicio;
        var mesesTotales = (int)duracionTotal.TotalDays / 30;
        var mitadDuracion = mesesTotales / 2;
        var pagosRealizados = await repoPago.ContarPagosPorActivos(idContrato);


        var diferencia = fechaRescision - fechaInicio;
        var mesesTranscurridos = (int)diferencia.TotalDays / 30;
        var deuda = mesesTranscurridos - pagosRealizados;

        decimal multa = contrato.Monto_mensual * deuda;
        if (mesesTranscurridos < mitadDuracion)
            multa += contrato.Monto_mensual * 2;
        else
            multa += contrato.Monto_mensual;

        return Json(new { multa });
    }
    [HttpGet]
    public IActionResult CrearDesdeContrato(int id)
    {
        var contratoViejo = repoContrato.ObtenerContratoPorId(id);
        if (contratoViejo == null)
        {
            TempData["MensajeError"] = "Contrato no encontrado.";
            return RedirectToAction("Index");
        }

        var inmueble = repoInmueble.ObtenerPorId(contratoViejo.Id_inmueble);
        var inquilino = repoInquilino.ObtenerInquilinoPorId(contratoViejo.Id_inquilino);

        var nuevoContrato = new Contrato
        {
            Id_inquilino = contratoViejo.Id_inquilino,
            Id_inmueble = contratoViejo.Id_inmueble,
            Monto_mensual = contratoViejo.Monto_mensual,
            Estado = "Vigente"
        };

        // Datos para mostrar en la vista Renovar
        ViewBag.InquilinoNombre = inquilino?.Nombre_completo ?? "Inquilino no disponible";
        ViewBag.InmuebleNombre = inmueble?.Direccion ?? "Inmueble no disponible";
        ViewBag.MontoMensual = contratoViejo.Monto_mensual;

        TempData["MensajeInfo"] = "Estás renovando un contrato. Completá las fechas.";

        return View("Renovar", nuevoContrato);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RenovarConfirmado(Contrato contrato)
    {
        // 🔐 Asignar usuario creador antes de validar
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null || !int.TryParse(claim.Value, out int idUsuario))
        {
            TempData["MensajeError"] = "No se pudo identificar al usuario creador.";
            return RedirectToAction("Index");
        }
        contrato.Id_usuario_creador = idUsuario;


        // Validación de fechas superpuestas
        if (repoContrato.ExisteContratoActivoEnRango(contrato.Id_inmueble, contrato.Fecha_inicio, contrato.Fecha_fin))
        {
            ModelState.AddModelError("", "Ya existe un contrato vigente en ese rango de fechas para este inmueble.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                repoContrato.CrearContrato(contrato);
                TempData["MensajeExito"] = "Contrato renovado correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error al renovar contrato: " + ex.Message;
            }
        }

        // reconstruir ViewBag si hay errores
        var inmueble = repoInmueble.ObtenerPorId(contrato.Id_inmueble);
        var inquilino = repoInquilino.ObtenerInquilinoPorId(contrato.Id_inquilino);

        ViewBag.InmuebleNombre = inmueble?.Direccion ?? "Inmueble no disponible";
        ViewBag.InquilinoNombre = inquilino?.Nombre_completo ?? "Inquilino no disponible";
        ViewBag.MontoMensual = contrato.Monto_mensual;
        

        return View("Renovar", contrato);
    }


}