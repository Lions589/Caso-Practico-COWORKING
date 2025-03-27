using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoworkingGestion.Models;
using System.Linq;

namespace CoworkingGestion.Controllers
{
    [Authorize(Roles = "Administrador")] // Solo los administradores pueden acceder a este controlador
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Página principal del panel de administración
        public IActionResult Index()
        {
            return View();
        }

        // Listar todos los usuarios registrados
        public IActionResult Usuarios()
        {
            var usuarios = _context.Usuarios.ToList();
            return View(usuarios);
        }

        // GET: Vista para crear un nuevo usuario
        [HttpGet]
        public IActionResult CrearUsuario()
        {
            return View();
        }

        // POST: Registrar un nuevo usuario con rol
        [HttpPost]
        public IActionResult CrearUsuario(string ncedula, string nombre, string apellido, string telefono, string email, string password, string rol)
        {
            // Verificar si el usuario ya existe
            var usuarioExistente = _context.Usuarios.FirstOrDefault(u => u.Email == email);

            if (usuarioExistente != null)
            {
                ViewBag.Error = "El correo ya está registrado.";
                return View();
            }

            // Crear nuevo usuario
            var nuevoUsuario = new Usuario
            {
                Ncedula = ncedula,
                Nombre = nombre,
                Apellido = apellido,
                Telefono = telefono,
                Email = email,
                PasswordHash = password, // ⚠ Aquí se puede implementar hashing en el futuro
                Rol = rol
            };

            _context.Usuarios.Add(nuevoUsuario);
            _context.SaveChanges();

            return RedirectToAction("Usuarios");
        }

        // Eliminar un usuario
        [HttpPost]
        public IActionResult EliminarUsuario(int id)
        {
            var usuario = _context.Usuarios.Find(id);

            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                _context.SaveChanges();
            }

            return RedirectToAction("Usuarios");
        }

        // Ver Reportes (Ejemplo de vista)
        public IActionResult Reportes()
        {
            return View();
        }
    }
}
