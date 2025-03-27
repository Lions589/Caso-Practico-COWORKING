using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoworkingGestion.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace CoworkingGestion.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Obtener la lista de espacios disponibles (por ejemplo, filtrados por estado "Disponible")
            var espaciosDisponibles = await _context.Espacios
                .Where(e => e.Estado == "Disponible")
                .ToListAsync();

            // Obtener las reservas del usuario, si está logueado (usando la sesión)
            int? userId = null;
            if (int.TryParse(HttpContext.Session.GetString("UserId"), out int id))
            {
                userId = id;
            }

            var reservasUsuario = userId.HasValue
                ? await _context.Reservas
                    .Include(r => r.IdEspacioNavigation)
                    .Where(r => r.IdUsuario == userId.Value)
                    .ToListAsync()
                : null;

            // Puedes crear un modelo de vista que contenga ambas listas o usar ViewBag/ViewData
            ViewBag.Espacios = espaciosDisponibles;
            ViewBag.Reservas = reservasUsuario;

            return View();
        }
    }
}
