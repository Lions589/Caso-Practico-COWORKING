using CoworkingGestion.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoworkingGestion.Controllers
{
    [Authorize]
    public class PlanesMembresiasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlanesMembresiasController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var planes = await _context.PlanesMembresias.ToListAsync();

            // Obtener el ID del usuario logeado
            var userIdStr = User.FindFirst("IdUsuario")?.Value;
            Membresia? membresiaUsuario = null;

            if (!string.IsNullOrEmpty(userIdStr))
            {
                int idUsuario = int.Parse(userIdStr);
                membresiaUsuario = await _context.Membresias
                    .Where(m => m.IdUsuario == idUsuario)
                    .OrderByDescending(m => m.IdMembresia) // última adquirida
                    .FirstOrDefaultAsync();
            }

            ViewBag.MembresiaUsuario = membresiaUsuario;
            return View(planes);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarPagoPlan(int idPlan)
        {
            var plan = await _context.PlanesMembresias.FindAsync(idPlan);
            if (plan == null)
            {
                TempData["ErrorMessage"] = "El plan no existe.";
                return RedirectToAction("Index");
            }

            var userIdStr = User.FindFirstValue("IdUsuario");
            if (string.IsNullOrEmpty(userIdStr))
            {
                TempData["ErrorMessage"] = "Usuario no autenticado.";
                return RedirectToAction("Index");
            }

            int idUsuario = int.Parse(userIdStr);

            var membresia = new Membresia
            {
                IdUsuario = idUsuario,
                NombrePlan = plan.NombrePlan,
                Precio = plan.Precio,
                Duracion = plan.Duracion,
                Descripcion = plan.Descripcion
            };

            _context.Membresias.Add(membresia);
            await _context.SaveChangesAsync();

            var pago = new Pago
            {
                IdMembresia = membresia.IdMembresia,
                Monto = plan.Precio,
                FechaPago = DateTime.Now,
                MetodoPago = "Pago Membresía"
            };

            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "¡Gracias por adquirir la membresía!";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Administrador")]
        // GET: Crear nuevo plan
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create(PlanesMembresia plan)
        {
            if (ModelState.IsValid)
            {
                _context.PlanesMembresias.Add(plan);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "¡Plan de membresía creado correctamente!";
                return RedirectToAction(nameof(Index));
            }

            return View(plan);
        }

    }
}
