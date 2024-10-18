using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using antigal.server.Custom;
using antigal.server.Models;
using antigal.server.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using antigal.server.Data;

namespace antigal.server.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous] // PONER EN TODO AQUELLO QUE SE PUEDE ACCEDER SIN ESTAR REGISTRADO
    [ApiController]
    public class AccesoController : ControllerBase
    {
        private readonly AppDbContext _AppDbContext;
        private readonly Utilidades _utilidades;

        public AccesoController(AppDbContext appDbContext, Utilidades utilidades)
        {
            _AppDbContext = appDbContext;
            _utilidades = utilidades;
        }

        [HttpPost]
        [Route("Registrarse")]
        public async Task<IActionResult> Registrarse(UsuarioDTO objeto)
        {
            var modeloUsuario = new Usuario
            {
                nombreCompleto = objeto.nombreCompleto,
                correo = objeto.correo,
                contrasenia = _utilidades.encriptarSHA256(objeto.contrasenia)
            };

            await _AppDbContext.Usuarios.AddAsync(modeloUsuario);
            await _AppDbContext.SaveChangesAsync();

            if (modeloUsuario.idUsuario != 0)
                return StatusCode(StatusCodes.Status200OK, new { isSucces = true });
            else
                return StatusCode(StatusCodes.Status200OK, new { isSucces = false });

        }
        /*
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginDTO objeto)
        {
            var usuarioEncontrado = await _AppDbContext:Usuarios
                                    .Where(u =>
                                        u.correo == objeto.correo &&
                                        u.contrasenia == _utilidades.encriptarSHA256(objeto.contrasenia)
                                    ).FirstOrDefaultAsync();
        }*/
    }
}
