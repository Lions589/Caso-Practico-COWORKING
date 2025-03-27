
using CoworkingGestion.Models;
using Microsoft.AspNetCore.Mvc;

public class ReportesController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReportesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var model = new ReportesViewModel
        {
            ReporteReservas = _context.VwReporteReservas.ToList(),
            ReportePagos = _context.VwReportePagos.ToList(),
            AnalisisUso = _context.VwAnalisisUsos.ToList()
        };

        return View(model);
    }
}