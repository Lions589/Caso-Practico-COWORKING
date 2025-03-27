using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoworkingGestion.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
// using CoworkingGestion.Filters; // Si lo necesitas para CustomAuthorize
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http; // Para HttpContext.Session

namespace CoworkingGestion.Controllers
{
    public class ReservasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reservas
        public async Task<IActionResult> Index()
        {
            // Incluye las propiedades de navegación para mostrar la información del usuario y espacio
            var reservas = await _context.Reservas
                .Include(r => r.IdUsuarioNavigation)
                .Include(r => r.IdEspacioNavigation)
                .ToListAsync();
            return View(reservas);
        }

        // GET: Reservas/Create
        public IActionResult Create()
        {
            // Cargar los espacios disponibles
            ViewBag.Espacios = _context.Espacios
                .Where(e => e.Estado == "Disponible")
                .ToList();
            return View();
        }

        // POST: Reservas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reserva reserva)
        {
            // Remover validaciones de navegación, ya que se asignan automáticamente
            ModelState.Remove("IdEspacioNavigation");
            ModelState.Remove("IdUsuarioNavigation");

            // Validar que se haya seleccionado un espacio válido
            if (reserva.IdEspacio == 0)
            {
                ModelState.AddModelError("IdEspacio", "Debe seleccionar un espacio válido.");
            }

            if (ModelState.IsValid)
            {
                // Asigna el usuario desde la sesión (asegúrate de haber guardado el UserId al iniciar sesión)
                var userIdString = HttpContext.Session.GetString("UserId");
                if (!int.TryParse(userIdString, out int userId))
                {
                    return Unauthorized();
                }
                reserva.IdUsuario = userId;

                // Asigna la fecha de reserva
                reserva.FechaReserva = DateTime.Now;

                // Consultar en la base de datos el espacio seleccionado para asignar su costo automáticamente
                var espacio = await _context.Espacios.FindAsync(reserva.IdEspacio);
                if (espacio != null)
                {
                    reserva.Costo = espacio.Costo;
                }
                else
                {
                    ModelState.AddModelError("IdEspacio", "El espacio seleccionado no existe.");
                    ViewBag.Espacios = _context.Espacios.Where(e => e.Estado == "Disponible").ToList();
                    return View(reserva);
                }

                // Establece un estado inicial, por ejemplo "Pendiente de Pago"
                reserva.Estado = "Pendiente de Pago";

                _context.Reservas.Add(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Si hay errores, recargar la lista de espacios
            ViewBag.Espacios = _context.Espacios.Where(e => e.Estado == "Disponible").ToList();
            return View(reserva);
        }



        // GET: Reservas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
                return NotFound();

            // Si este dropdown se usa solo para administradores, asegúrate de asignarlo
            ViewData["Usuarios"] = _context.Usuarios.ToList();  // Asegúrate de que no sea null
            ViewData["Espacios"] = _context.Espacios.ToList();
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Reserva reserva)
        {
            if (id != reserva.IdReserva)
                return NotFound();

            // Remover las validaciones de las propiedades de navegación
            ModelState.Remove("IdUsuarioNavigation");
            ModelState.Remove("IdEspacioNavigation");

            // Validar que se haya seleccionado un espacio válido
            if (reserva.IdEspacio == 0)
                ModelState.AddModelError("IdEspacio", "Debe seleccionar un espacio válido.");

            // Nota: IdUsuario no se cambia manualmente si es un cliente;
            //       si el rol es Admin, podrías permitirlo. Ajusta según tu lógica.

            if (ModelState.IsValid)
            {
                var reservaDb = await _context.Reservas.FirstOrDefaultAsync(r => r.IdReserva == id);
                if (reservaDb == null)
                    return NotFound();

                // Actualizar solo los campos editables; conserva la FechaReserva e IdUsuario
                // si no quieres que el usuario la cambie
                reservaDb.IdEspacio = reserva.IdEspacio;
                reservaDb.FechaInicio = reserva.FechaInicio;
                reservaDb.FechaFin = reserva.FechaFin;
                reservaDb.Estado = reserva.Estado;
                reservaDb.Costo = reserva.Costo;
                reservaDb.CodigoQr = reserva.CodigoQr;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.IdReserva))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["Espacios"] = _context.Espacios.ToList();
            return View(reserva);
        }

        // GET: Reservas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var reserva = await _context.Reservas
                .Include(r => r.IdUsuarioNavigation)
                .Include(r => r.IdEspacioNavigation)
                .FirstOrDefaultAsync(m => m.IdReserva == id);
            if (reserva == null)
                return NotFound();

            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }

            // Verificar si existen pagos asociados a la reserva
            bool tienePagos = _context.Pagos.Any(p => p.IdReserva == id);
            if (tienePagos)
            {
                TempData["ErrorMessage"] = "No se puede eliminar la reserva porque tiene pagos asociados.";
                return RedirectToAction(nameof(Index));
            }

            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(e => e.IdReserva == id);
        }

        [HttpGet]
        public async Task<IActionResult> GetReservasCalendario()
        {
            var reservas = await _context.Reservas
                .Include(r => r.IdEspacioNavigation)
                .Where(r => r.Estado == "Pagado") // Solo mostrar reservas pagadas
                .Select(r => new
                {
                    title = r.IdEspacioNavigation.Nombre, // Nombre del espacio reservado
                    start = r.FechaInicio.ToString("yyyy-MM-ddTHH:mm:ss"),
                    end = r.FechaFin.ToString("yyyy-MM-ddTHH:mm:ss"),
                    backgroundColor = "green", // Color del evento en el calendario
                    borderColor = "darkgreen"
                })
                .ToListAsync();

            if (reservas == null || !reservas.Any())
            {
                return Json(new List<object>()); // Devolver una lista vacía si no hay reservas
            }

            return Json(reservas);
        }

    }
}
