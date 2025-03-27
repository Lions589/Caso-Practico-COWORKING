using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoworkingGestion.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace CoworkingGestion.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Dashboard/Index
        public async Task<IActionResult> Index()
        {
            // Obtener el email del usuario autenticado
            var userEmail = User.Identity.Name;
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (usuario == null)
            {
                return RedirectToAction("Login", "Account"); // Si el usuario no existe, redirigir al login
            }

            int userId = usuario.IdUsuario; // Obtener el ID del usuario

            // Consultar las reservas del usuario
            var reservas = await _context.Reservas
                .Include(r => r.IdEspacioNavigation)
                .Where(r => r.IdUsuario == userId)
                .ToListAsync();

            // Consultar las membresías del usuario
            var membresias = await _context.Membresias
                .Include(m => m.IdUsuarioNavigation)
                .Where(m => m.IdUsuario == userId)
                .ToListAsync();

            var viewModel = new DashboardViewModel
            {
                Reservas = reservas,
                Membresias = membresias
            };

            

            return View(viewModel);
        }
    }
}
