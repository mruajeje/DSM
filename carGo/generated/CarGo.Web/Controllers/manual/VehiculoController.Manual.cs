using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace CarGo.Web.Controllers;

public partial class VehiculoController
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubirFoto(int p_oid, IFormFile? foto)
    {
        if (foto == null || foto.Length == 0)
        {
            TempData["ErrorMessage"] = "Selecciona una imagen válida antes de subirla.";
            return RedirectToAction(nameof(Details), new { id = p_oid });
        }

        var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var ext = Path.GetExtension(foto.FileName).ToLowerInvariant();
        if (!extensionesPermitidas.Contains(ext))
        {
            TempData["ErrorMessage"] = "Formato no permitido. Usa JPG, PNG o WEBP.";
            return RedirectToAction(nameof(Details), new { id = p_oid });
        }

        var carpetaUploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "vehiculos");
        Directory.CreateDirectory(carpetaUploads);

        // Borrar foto anterior si tuviera otra extensión
        foreach (var antiguo in Directory.GetFiles(carpetaUploads, $"vehiculo_{p_oid}.*"))
        {
            System.IO.File.Delete(antiguo);
        }

        var rutaDestino = Path.Combine(carpetaUploads, $"vehiculo_{p_oid}{ext}");
        await using (var stream = new FileStream(rutaDestino, FileMode.Create))
        {
            await foto.CopyToAsync(stream);
        }

        TempData["SuccessMessage"] = "Foto del vehículo subida correctamente.";
        return RedirectToAction(nameof(Details), new { id = p_oid });
    }
}