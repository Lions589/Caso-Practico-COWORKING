using CoworkingGestion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CoworkingGestion.Controllers
{
    public class EspaciosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EspaciosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var espacios = await _context.Espacios.ToListAsync();
            return View(espacios);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Espacio espacio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(espacio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(espacio);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var espacio = await _context.Espacios.FindAsync(id);
            if (espacio == null)
                return NotFound();

            return View(espacio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Espacio espacio)
        {
            if (id != espacio.IdEspacio)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(espacio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(espacio);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var espacio = await _context.Espacios
                .FirstOrDefaultAsync(m => m.IdEspacio == id);
            if (espacio == null)
                return NotFound();

            return View(espacio);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var espacio = await _context.Espacios.FindAsync(id);
            _context.Espacios.Remove(espacio);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var espacio = await _context.Espacios.FirstOrDefaultAsync(e => e.IdEspacio == id);
            if (espacio == null)
                return NotFound();

            return View(espacio);
        }
    }
}
