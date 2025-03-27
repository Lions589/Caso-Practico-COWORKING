using CoworkingGestion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoworkingGestion.Controllers
{
    public class TestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Acción para crear (Insertar) un nuevo usuario
        [HttpGet]
        public async Task<IActionResult> CreateUsuario()
        {
            var usuario = new Usuario
            {
                Nombre = "Juan",
                Apellido = "Perez",
                Email = "juan.perez@example.com",
                PasswordHash = "dummyhash",
                Rol = "Miembro",
                Ncedula = "12345678",
                Telefono = "555-1234"
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return Content($"Usuario creado con ID: {usuario.IdUsuario}");
        }

        // Acción para leer (listar) todos los usuarios
        [HttpGet]
        public async Task<IActionResult> ReadUsuarios()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            string output = "<h2>Lista de Usuarios:</h2>";
            foreach (var user in usuarios)
            {
                output += $"ID: {user.IdUsuario}, Nombre: {user.Nombre} {user.Apellido}<br/>";
            }
            return Content(output, "text/html");
        }

        // Acción para actualizar un usuario (por ejemplo, el primer usuario de la lista)
        [HttpGet]
        public async Task<IActionResult> UpdateUsuario()
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync();
            if (usuario != null)
            {
                usuario.Nombre = "Actualizado";
                await _context.SaveChangesAsync();
                return Content($"Usuario actualizado: ID {usuario.IdUsuario}");
            }
            return Content("No hay usuario para actualizar.");
        }

        // Acción para eliminar un usuario (por ejemplo, el primer usuario de la lista)
        [HttpGet]
        public async Task<IActionResult> DeleteUsuario()
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync();
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
                return Content($"Usuario eliminado: ID {usuario.IdUsuario}");
            }
            return Content("No hay usuario para eliminar.");
        }
    }
}
