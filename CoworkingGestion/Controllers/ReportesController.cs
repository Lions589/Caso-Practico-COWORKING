
using CoworkingGestion.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CoworkingGestion.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ReportesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ReporteReservas()
        {
            var datos = await _context.VwReporteReservas.ToListAsync();
            return View(datos);
        }

        public async Task<IActionResult> ReportePagos()
        {
            var datos = await _context.VwReportePagos.ToListAsync();
            return View(datos);
        }

        public async Task<IActionResult> AnalisisUso()
        {
            var datos = await _context.VwAnalisisUsos.ToListAsync();
            return View(datos);
        }
    }
}
