using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SIGDEF.AccesoDatos;
using SIGDEF.Entidades;
using SIGDEF.Services;

namespace SIGDEF.Controllers // Ajusta según tu proyecto
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentacionController : ControllerBase
    {
        private readonly Cloudinary _cloudinary;
        private readonly SIGDeFContext _context;

        public DocumentacionController(CloudinaryService cloudinaryService, SIGDeFContext context)
        {
            _context = context;
            _cloudinary = cloudinaryService.GetCloudinary();
        }

        [HttpPost("upload")]
        [RequestSizeLimit(10 * 1024 * 1024)] // 10MB
        public async Task<IActionResult> Upload([FromForm] DocumentoUploadDto dto)
        {
         
            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("No se envió ningún archivo.");

            // Validar que la persona existe antes de subir nada
            var personaExiste = await _context.Personas.AnyAsync(p => p.IdPersona == dto.PersonaId);
            if (!personaExiste)
                return NotFound($"No existe la Persona con ID {dto.PersonaId}");

            // 2. Subir a Cloudinary
            var uploadResult = new ImageUploadResult();

            try
            {
                using (var stream = dto.File.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(dto.File.FileName, stream),
                        Folder = $"sigdef/documentacion/{dto.PersonaId}",
                        PublicId = $"{dto.TipoDocumento}_{Guid.NewGuid()}",
                        Overwrite = true
                    };
                    uploadResult = await _cloudinary.UploadAsync(uploadParams);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error conectando con Cloudinary: {ex.Message}");
            }

            if (uploadResult.Error != null)
                return StatusCode(500, uploadResult.Error.Message);

            // 3. Guardar o Actualizar referencia en Base de Datos
            var documentoExistente = await _context.DocumentacionPersonas
                .FirstOrDefaultAsync(d => d.PersonaId == dto.PersonaId && d.TipoDocumento == dto.TipoDocumento);

            if (documentoExistente != null)
            {
                // Actualizamos registro existente
                documentoExistente.UrlArchivo = uploadResult.SecureUrl.ToString();
                documentoExistente.PublicId = uploadResult.PublicId;
                documentoExistente.FechaCarga = DateTime.UtcNow;
                _context.Update(documentoExistente);
            }
            else
            {
                // Creamos nuevo registro
                var nuevaDocumentacion = new DocumentacionPersona
                {
                    PersonaId = dto.PersonaId,
                    TipoDocumento = dto.TipoDocumento,
                    UrlArchivo = uploadResult.SecureUrl.ToString(),
                    PublicId = uploadResult.PublicId,
                    FechaCarga = DateTime.UtcNow
                };
                _context.DocumentacionPersonas.Add(nuevaDocumentacion);
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                url = uploadResult.SecureUrl.ToString(),
                message = "Documento subido correctamente"
            });
        }

        // Endpoint para obtener documentos de una persona
        [HttpGet("persona/{personaId}")]
        public async Task<IActionResult> GetByPersona(int personaId)
        {
            var docs = await _context.DocumentacionPersonas
                .Where(d => d.PersonaId == personaId)
                .Select(d => new {
                    d.Id,
                    d.TipoDocumento,
                    d.UrlArchivo,
                    d.FechaCarga
                })
                .ToListAsync();

            return Ok(docs);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Buscar el documento
                var documento = await _context.DocumentacionPersonas
                    .FirstOrDefaultAsync(d => d.Id == id);
                if (documento == null)
                    return NotFound($"No se encontró el documento con ID {id}");
                // Eliminar de Cloudinary si existe PublicId
                if (!string.IsNullOrEmpty(documento.PublicId))
                {
                    try
                    {
                        var deleteParams = new DeletionParams(documento.PublicId);
                        var deleteResult = await _cloudinary.DestroyAsync(deleteParams);

                        if (deleteResult.Result != "ok" && deleteResult.Result != "not found")
                        {
                            Console.WriteLine($"⚠️ Advertencia: No se pudo eliminar de Cloudinary: {deleteResult.Error?.Message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Error al eliminar de Cloudinary: {ex.Message}");
                        // Continuamos con la eliminación de la BD aunque falle Cloudinary
                    }
                }
                // Eliminar de la base de datos
                _context.DocumentacionPersonas.Remove(documento);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Documento eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar documento: {ex.Message}");
            }
        }
    }

    // DTO para recibir los datos del Front
    public class DocumentoUploadDto
    {
        public IFormFile File { get; set; }
        public int PersonaId { get; set; }
        public int TipoDocumento { get; set; }
    }

}
