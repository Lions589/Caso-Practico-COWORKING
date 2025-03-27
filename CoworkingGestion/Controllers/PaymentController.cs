using Microsoft.AspNetCore.Mvc;
using CoworkingGestion.Models;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using SkiaSharp.QrCode;
using System.IO;
using SkiaSharp.QrCode.Image;

namespace CoworkingGestion.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Acción GET para mostrar la vista de pago de una reserva
        [HttpGet]
        public async Task<IActionResult> Pagar(int reservaId)
        {
            var reserva = await _context.Reservas
                .Include(r => r.IdEspacioNavigation)
                .FirstOrDefaultAsync(r => r.IdReserva == reservaId);

            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RealizarPago(int reservaId)
        {
            var reserva = await _context.Reservas.FindAsync(reservaId);
            if (reserva == null)
            {
                return NotFound();
            }

            // Simula el pago: actualiza el estado a "Pagado"
            reserva.Estado = "Pagado";

            // Generar y asignar QR
            GenerarQRParaReserva(reserva);

            // Registrar la transacción en la tabla de Pagos
            var pago = new Pago
            {
                IdReserva = reserva.IdReserva,
                Monto = reserva.Costo,
                FechaPago = DateTime.Now,
                MetodoPago = "Pago Artificial"
            };

            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "GRACIAS POR SU PAGO, su reservación se realizó con éxito. Aquí está su QR de acceso.";

            return RedirectToAction("Index", "Reservas");
        }

        // ✅ Acción para generar QR manualmente si la reserva está pagada
        [HttpGet]
        public async Task<IActionResult> GenerarQR(int reservaId)
        {
            var reserva = await _context.Reservas.FindAsync(reservaId);
            if (reserva == null)
            {
                TempData["ErrorMessage"] = "Reserva no encontrada.";
                return RedirectToAction("Index", "Reservas");
            }

            if (reserva.Estado.ToLower() != "pagado")
            {
                TempData["ErrorMessage"] = "Debe pagar la reserva antes de generar el QR.";
                return RedirectToAction("Index", "Reservas");
            }

            // Generar y asignar QR
            GenerarQRParaReserva(reserva);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "QR generado correctamente.";

            return RedirectToAction("Index", "Reservas");
        }

        // ✅ Método centralizado para generar QR
        private void GenerarQRParaReserva(Reserva reserva)
        {
            if (string.IsNullOrEmpty(reserva.CodigoQr))
            {
                reserva.CodigoQr = Guid.NewGuid().ToString(); // Token único si no existe
            }

            string qrCodeBase64 = GenerateQrCodeBase64(reserva.CodigoQr);
            TempData["QrImage"] = qrCodeBase64;
        }

        // ✅ Endpoint para validar el acceso con el token QR
        [HttpGet]
        public async Task<IActionResult> ValidarQR(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("El token es requerido.");
            }

            var reserva = await _context.Reservas.FirstOrDefaultAsync(r => r.CodigoQr == token);
            if (reserva == null)
            {
                return NotFound("Reserva no encontrada o QR inválido.");
            }

            // Verificar si la reserva está pagada
            if (!string.Equals(reserva.Estado, "Pagado", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Acceso no autorizado: Reserva no pagada.");
            }

            return Ok(new { message = "Acceso autorizado.", reservaId = reserva.IdReserva, estado = reserva.Estado });
        }

        // ✅ Método para generar QR con SkiaSharp
        private string GenerateQrCodeBase64(string token)
        {
            var qrCode = new QrCode(token, new Vector2Slim(256, 256), SKEncodedImageFormat.Png);
            using (var ms = new MemoryStream())
            {
                qrCode.GenerateImage(ms);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadIcs(int reservaId)
        {
            var reserva = await _context.Reservas.FindAsync(reservaId);
            if (reserva == null)
            {
                return NotFound();
            }
            string icsContent = GenerateIcsFileContent(reserva);
            byte[] icsBytes = Encoding.UTF8.GetBytes(icsContent);
            return File(icsBytes, "text/calendar", "reserva.ics");
        }

      

        private string GenerateIcsFileContent(Reserva reserva)
        {
            return $@"BEGIN:VCALENDAR
VERSION:2.0
PRODID:-//CoworkingGestion//Reservation//EN
BEGIN:VEVENT
UID:{reserva.IdReserva}@coworkinggestion.com
DTSTAMP:{DateTime.UtcNow:yyyyMMddTHHmmssZ}
DTSTART:{reserva.FechaInicio:yyyyMMddTHHmmssZ}
DTEND:{reserva.FechaFin:yyyyMMddTHHmmssZ}
SUMMARY:Reserva #{reserva.IdReserva}
DESCRIPTION:Reserva en estado {reserva.Estado} por un costo de {reserva.Costo:C}
END:VEVENT
END:VCALENDAR";
        }
    }
}
