using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SIGDEF.AccesoDatos;
using SIGDEF.Entidades;
using SIGDEF.Services;

namespace SIGDEF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentacionController : ControllerBase
    {
        private readonly Cloudinary _cloudinary;
        private readonly SIGDeFContext _context;
        private readonly ILogger<DocumentacionController> _logger;

        public DocumentacionController(
            CloudinaryService cloudinaryService,
            SIGDeFContext context,
            ILogger<DocumentacionController> logger)
        {
            _context = context;
            _cloudinary = cloudinaryService.GetCloudinary();
            _logger = logger;
        }

        [HttpPost("upload")]
        [RequestSizeLimit(10 * 1024 * 1024)] // 10MB - Removido DisableRequestSizeLimit
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload(
            [FromForm] DocumentoUploadDto dto,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("🔵 Iniciando upload de documento...");

            try
            {
                // 🔹 Validación temprana de cancelación
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogWarning("⚠️ Petición cancelada antes de procesar");
                    return StatusCode(499, new { error = "Petición cancelada por el cliente" });
                }

                // 🔹 LOG DE DEBUG - Ver qué llega
                _logger.LogInformation($"📦 DTO recibido - PersonaId: {dto.PersonaId}, TipoDocumento: {dto.TipoDocumento}, File: {dto.File?.FileName ?? "NULL"}");

                // Validación 1: PersonaId válido
                //if (!dto.PersonaId || dto.PersonaId. <= 0)
                //{
                //    _logger.LogWarning($"⚠️ PersonaId inválido o vacío: {dto.PersonaId}");
                //    return BadRequest(new { error = "PersonaId es requerido y debe ser mayor a 0" });
                //}

                //// Validación 2: TipoDocumento válido
                //if (!dto.TipoDocumento || dto.TipoDocumento <= 0)
                //{
                //    _logger.LogWarning($"⚠️ TipoDocumento inválido o vacío: {dto.TipoDocumento}");
                //    return BadRequest(new { error = "TipoDocumento es requerido y debe ser mayor a 0" });
                //}

                // Validación 3: Archivo existe y tiene contenido
                if (dto.File == null)
                {
                    _logger.LogWarning("⚠️ File es NULL");
                    return BadRequest(new { error = "No se envió ningún archivo." });
                }

                if (dto.File.Length == 0)
                {
                    _logger.LogWarning("⚠️ File.Length es 0");
                    return BadRequest(new { error = "El archivo está vacío." });
                }

                // Validación 4: Tamaño máximo
                if (dto.File.Length > 10485760) // 10MB
                {
                    _logger.LogWarning($"⚠️ Archivo muy grande: {dto.File.Length} bytes");
                    return BadRequest(new { error = "El archivo no puede superar los 10MB" });
                }

                // Validación 5: Tipo de archivo permitido
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx" };
                var extension = Path.GetExtension(dto.File.FileName)?.ToLowerInvariant();

                if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                {
                    _logger.LogWarning($"⚠️ Extensión no permitida: {extension}");
                    return BadRequest(new
                    {
                        error = "Formato no permitido. Permitidos: jpg, jpeg, png, gif, pdf, doc, docx"
                    });
                }

                _logger.LogInformation($"📄 Archivo recibido: {dto.File.FileName} ({dto.File.Length} bytes)");

                // Validación 6: La persona existe
                var personaExiste = await _context.Personas
                    .AsNoTracking()
                    .AnyAsync(p => p.IdPersona == dto.PersonaId, cancellationToken);

                if (!personaExiste)
                {
                    _logger.LogWarning($"⚠️ Persona no encontrada: {dto.PersonaId}");
                    return NotFound(new { error = $"No existe la Persona con ID {dto.PersonaId}" });
                }

                _logger.LogInformation($"📤 Subiendo a Cloudinary...");

                // Subir a Cloudinary con manejo robusto de errores
                UploadResult uploadResult;

                try
                {
                    // Copiar el stream a un MemoryStream para evitar problemas de conexión
                    using var memoryStream = new MemoryStream();
                    await dto.File.CopyToAsync(memoryStream, cancellationToken);
                    memoryStream.Position = 0;

                    // Verificar cancelación después de copiar
                    if (cancellationToken.IsCancellationRequested)
                    {
                        _logger.LogWarning("⚠️ Upload cancelado después de leer archivo");
                        return StatusCode(499, new { error = "Operación cancelada" });
                    }

                    // Subir a Cloudinary según el tipo de archivo
                    if (extension == ".pdf" || extension == ".doc" || extension == ".docx")
                    {
                        var uploadParams = new RawUploadParams()
                        {
                            File = new FileDescription(dto.File.FileName, memoryStream),
                            Folder = $"sigdef/documentacion/{dto.PersonaId}",
                            PublicId = $"{dto.TipoDocumento}_{Guid.NewGuid()}",
                            Overwrite = true
                        };
                        uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    }
                    else // Para imágenes
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(dto.File.FileName, memoryStream),
                            Folder = $"sigdef/documentacion/{dto.PersonaId}",
                            PublicId = $"{dto.TipoDocumento}_{Guid.NewGuid()}",
                            Overwrite = true
                        };
                        uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("⚠️ Operación cancelada durante upload");
                    return StatusCode(499, new { error = "Operación cancelada" });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error al subir a Cloudinary");
                    return StatusCode(500, new
                    {
                        error = "Error al subir el archivo",
                        details = ex.Message
                    });
                }

                if (uploadResult?.Error != null)
                {
                    _logger.LogError($"❌ Error en Cloudinary: {uploadResult.Error.Message}");
                    return StatusCode(500, new
                    {
                        error = "Error al subir a Cloudinary",
                        details = uploadResult.Error.Message
                    });
                }

                _logger.LogInformation($"✅ Archivo subido a Cloudinary: {uploadResult.SecureUrl}");

                // Guardar o actualizar en base de datos
                var documentoExistente = await _context.DocumentacionPersonas
                    .FirstOrDefaultAsync(d => d.PersonaId == dto.PersonaId
                        && d.TipoDocumento == dto.TipoDocumento,
                        cancellationToken);

                if (documentoExistente != null)
                {
                    _logger.LogInformation($"🔄 Actualizando documento existente ID: {documentoExistente.Id}");

                    // Eliminar el archivo anterior de Cloudinary
                    if (!string.IsNullOrEmpty(documentoExistente.PublicId))
                    {
                        try
                        {
                            var deleteParams = new DeletionParams(documentoExistente.PublicId);
                            await _cloudinary.DestroyAsync(deleteParams);
                            _logger.LogInformation($"🗑️ Archivo anterior eliminado de Cloudinary");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning($"⚠️ No se pudo eliminar archivo anterior: {ex.Message}");
                        }
                    }

                    documentoExistente.UrlArchivo = uploadResult.SecureUrl.ToString();
                    documentoExistente.PublicId = uploadResult.PublicId;
                    documentoExistente.FechaCarga = DateTime.UtcNow;
                    _context.Update(documentoExistente);
                }
                else
                {
                    _logger.LogInformation($"➕ Creando nuevo documento");

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

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation($"✅ Documento guardado exitosamente en BD");

                return Ok(new
                {
                    success = true,
                    url = uploadResult.SecureUrl.ToString(),
                    publicId = uploadResult.PublicId,
                    tipoDocumento = dto.TipoDocumento,
                    personaId = dto.PersonaId,
                    message = "Documento subido correctamente"
                });
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("⚠️ Operación cancelada por el cliente");
                return StatusCode(499, new { error = "Operación cancelada" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error general en Upload");
                return StatusCode(500, new
                {
                    error = "Error al procesar el archivo",
                    details = ex.Message
                });
            }
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            _logger.LogInformation("✅ Test endpoint llamado");
            return Ok(new { message = "API funcionando correctamente", timestamp = DateTime.UtcNow });
        }

        [HttpPost("test-upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> TestUpload([FromForm] IFormFile file)
        {
            _logger.LogInformation("🧪 TEST UPLOAD SIMPLE");

            if (file == null)
                return BadRequest(new { error = "No file received" });

            _logger.LogInformation($"File: {file.FileName} - {file.Length} bytes");

            return Ok(new
            {
                success = true,
                fileName = file.FileName,
                size = file.Length,
                contentType = file.ContentType
            });
        }

        [HttpGet("persona/{personaId}")]
        public async Task<IActionResult> GetByPersona(int personaId)
        {
            try
            {
                if (personaId <= 0)
                    return BadRequest(new { error = "PersonaId debe ser mayor a 0" });

                var docs = await _context.DocumentacionPersonas
                    .Where(d => d.PersonaId == personaId)
                    .Select(d => new {
                        d.Id,
                        d.TipoDocumento,
                        d.UrlArchivo,
                        d.PublicId,
                        d.FechaCarga
                    })
                    .OrderByDescending(d => d.FechaCarga)
                    .AsNoTracking()
                    .ToListAsync();

                return Ok(new { success = true, documentos = docs, total = docs.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener documentos de persona {personaId}");
                return StatusCode(500, new { error = "Error al obtener documentos", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation($"🗑️ Intentando eliminar documento ID: {id}");

                var documento = await _context.DocumentacionPersonas
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (documento == null)
                {
                    _logger.LogWarning($"⚠️ Documento no encontrado: {id}");
                    return NotFound(new { error = $"No se encontró el documento con ID {id}" });
                }

                // Eliminar de Cloudinary
                if (!string.IsNullOrEmpty(documento.PublicId))
                {
                    try
                    {
                        var deleteParams = new DeletionParams(documento.PublicId);
                        var deleteResult = await _cloudinary.DestroyAsync(deleteParams);

                        if (deleteResult.Result == "ok" || deleteResult.Result == "not found")
                        {
                            _logger.LogInformation($"✅ Archivo eliminado de Cloudinary");
                        }
                        else
                        {
                            _logger.LogWarning($"⚠️ Respuesta inesperada de Cloudinary: {deleteResult.Result}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"⚠️ Error al eliminar de Cloudinary: {ex.Message}");
                    }
                }

                _context.DocumentacionPersonas.Remove(documento);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"✅ Documento eliminado de BD correctamente");

                return Ok(new
                {
                    success = true,
                    message = "Documento eliminado correctamente",
                    id = id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar documento {id}");
                return StatusCode(500, new
                {
                    error = "Error al eliminar documento",
                    details = ex.Message
                });
            }
        }
    }

    public class DocumentoUploadDto
    {
        public IFormFile File { get; set; }
        public int PersonaId { get; set; }
        public int TipoDocumento { get; set; }
    }
}