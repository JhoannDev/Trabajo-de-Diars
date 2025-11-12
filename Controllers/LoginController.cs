using Microsoft.AspNetCore.Mvc;
using Proyecto.Services;
using Proyecto.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
namespace Proyecto.Controllers
{
    public class LoginController : Controller
    {
        private readonly UsuarioService _usuarioService;
        public LoginController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
            _usuarioService.CrearAdminPorDefectoAsync().Wait(); // crear admin si no existe
        }

        // Muestra el formulario
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Procesa el formulario
        [HttpPost]
        public async Task<IActionResult> Index(string email, string password)
        {
            var user = await _usuarioService.ValidarLoginAsync(email, password);

            if (user != null)
            {
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserName", user.Nombre);
                HttpContext.Session.SetString("UserRole", user.Rol);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Correo o contraseña incorrectos.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // Recuperación de contraseña
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var token = await _usuarioService.GenerarTokenRecuperacionAsync(email);
            var link = Url.Action("ResetPassword", "Login", new { token }, Request.Scheme);
            await _usuarioService.EnviarEmailAsync(email, "Recuperación de Contraseña", $"Haz clic para restablecer: {link}");
            ViewBag.Message = "Se envió un enlace de recuperación a tu correo.";
            return View();
        }

        // Restablecimiento de contraseña
        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string token, string nueva, string confirmar)
        {
            if (nueva != confirmar)
            {
                ViewBag.Error = "Las contraseñas no coinciden.";
                return View();
            }

            if (!await _usuarioService.ValidarTokenAsync(token))
            {
                ViewBag.Error = "Token inválido o expirado.";
                return View();
            }

            var email = await _usuarioService.ObtenerEmailPorTokenAsync(token);
            if (email == null)
            {
                ViewBag.Error = "No se encontró un usuario asociado al token.";
                return View();
            }

            await _usuarioService.CambiarPasswordAsync(email, nueva);
            await _usuarioService.MarcarTokenUsadoAsync(token);

            ViewBag.Message = "Contraseña restablecida correctamente.";
            return View();
        }
    }
}
