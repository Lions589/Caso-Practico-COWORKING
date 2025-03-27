using CoworkingGestion.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoworkingGestion.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Vista de Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Validar usuario y contraseña
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Buscar usuario en la base de datos
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email && u.PasswordHash == password);

            if (usuario != null)
            {
                // Crear Claims para la autenticación, incluyendo el IdUsuario
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Email),
                    new Claim("IdUsuario", usuario.IdUsuario.ToString()),
                    new Claim(ClaimTypes.Role, usuario.Rol) // Guardar el rol del usuario
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Guardar el ID del usuario en sesión (opcional si ya está en los claims)
                HttpContext.Session.SetString("UserId", usuario.IdUsuario.ToString());

                // Iniciar sesión del usuario
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Redirigir según el rol del usuario
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Error = "Correo o contraseña incorrectos.";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string nombre, string apellido, string telefono, string email, string password, string rol)
        {
            var usuarioExistente = _context.Usuarios.FirstOrDefault(u => u.Email == email);

            if (usuarioExistente != null)
            {
                ViewBag.Error = "El correo ya está registrado.";
                return View();
            }

            var nuevoUsuario = new Usuario
            {
                Nombre = nombre,
                Apellido = apellido,
                Telefono = telefono,
                Email = email,
                PasswordHash = password,
                Rol = rol
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }
    }
}
