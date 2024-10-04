using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO; 
using System.Threading.Tasks; 
using antigal.server.Models;
using antigal.server.Data; 
using Microsoft.EntityFrameworkCore;

namespace antigal.server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagenesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ImagenesController(AppDbContext context)
        {
            _context = context;
        }

        // GET api/imagenes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Imagen>>> GetImagenes()
        {
            return await _context.Imagenes.ToListAsync();
        }

        // GET api/imagenes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Imagen>> GetImagen(int id)
        {
            var imagen = await _context.Imagenes.FindAsync(id);
            if (imagen == null)
            {
                return NotFound();
            }
            return imagen;
        }

        // POST api/imagenes
        [HttpPost]
        public async Task<ActionResult<Imagen>> PostImagen([FromForm] Imagen imagen, IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file was uploaded.");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest("Invalid file type.");
            }

            // Generate a unique filename to avoid overwriting
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "images", fileName);

            // Save the image to local storage
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Update the imagen object with a public path, not system path
            imagen.url = Path.Combine("images", fileName);

            _context.Imagenes.Add(imagen);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetImagen), new { id = imagen.id }, imagen);
        }


        // DELETE api/imagenes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteImagen(int id)
        {
            var imagen = await _context.Imagenes.FindAsync(id);
            if (imagen == null)
            {
                return NotFound();
            }

            // Remove the image from local storage
            var filePath = imagen.url;
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.Imagenes.Remove(imagen);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}