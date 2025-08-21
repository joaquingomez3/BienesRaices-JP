using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using bienesraices.Models;
using bienesraices.Repositorios;

namespace bienesraices.Controllers
{
    public class LoginController : Controller
    {
        private readonly RepositorioUsuario repo;

        public LoginController()
        {
            repo = new RepositorioUsuario();
        }

        // GET: Login
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public IActionResult Index(string email, string password)
        {
            var usuario = repo.ObtenerUsuario(email, password);

            if (usuario != null)
            {
                // Guardamos en sesión
                HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
                HttpContext.Session.SetString("NombreUsuario", usuario.Nombre_usuario);
                HttpContext.Session.SetInt32("TipoUsuario", usuario.Id_tipo_usuario);

                // Redirige a la página de inicio
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = "Email o contraseña incorrectos.";
                return View();
            }
        }

        // Cerrar sesión
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}