using CoworkingGestion.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CoworkingGestion.Controllers
{
    [Authorize]
    public class MembresiasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MembresiasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Mostrar las membresías del usuario logueado
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirst("IdUsuario")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                TempData["ErrorMessage"] = "Usuario no autenticado.";
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdClaim);

            var membresias = await _context.Membresias
                .Where(m => m.IdUsuario == userId)
                .ToListAsync();

            return View(membresias);
        }

        // Vista para crear membresía (solo admin)
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            ViewBag.Usuarios = new SelectList(_context.Usuarios.ToList(), "IdUsuario", "Email");
            return View();
        }

        // Crear membresía desde formulario
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create(Membresia membresia)
        {
            if (ModelState.IsValid)
            {
                _context.Membresias.Add(membresia);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Membresía creada correctamente.";
                return RedirectToAction("Index");
            }

            ViewBag.Usuarios = new SelectList(_context.Usuarios.ToList(), "IdUsuario", "Email");
            return View(membresia);
        }
    }
}
