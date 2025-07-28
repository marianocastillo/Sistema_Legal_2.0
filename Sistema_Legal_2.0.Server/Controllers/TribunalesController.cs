using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExcelDataReader;
using System.Data;
using System.Reflection;
using System.Numerics;
using Dapper;
using Sistema_Legal_2._0.Server.Models;
using Sistema_Legal_2._0.Server.Repositories;
using Sistema_Legal_2._0.Server.Infraestructure;
using Sistema_Legal_2._0.Server.Entities;
using System.Configuration;
using Microsoft.Data.SqlClient;
using MimeMapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.CodeAnalysis;
using System.Linq;

namespace Sistema_Legal_2._0.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TribunalesController : ControllerBase
    {

        private readonly string _filesServerPath;
        private readonly string _cadenaSQL;
        private IConfiguration configuration;
        private readonly IConfiguration _configuration;
        private readonly Logger _logger;
        private readonly db_silegContext _db_silegContext;
        public TribunalesController(IConfiguration config, db_silegContext db_SilegContext)
        {
            _db_silegContext = db_SilegContext;
            _filesServerPath = config.GetSection("Configuracion").GetSection("FilesServerPath").Value;
            _cadenaSQL = config.GetConnectionString("Sistema_Legal");
            _configuration = config;
        }
        /// <summary>
        /// Trae todos los tribunales
        /// </summary>
        /// <returns></returns>
        [HttpGet("Tribunales")]
        public async Task<IActionResult> GetTodos()
        {
            using var connection = new SqlConnection(_cadenaSQL);
            var tribunales = await connection.QueryAsync<Tribunales>("SELECT * FROM Tribunales");
            return Ok(tribunales);
        }

        /// <summary>
        /// Obtener un tribunal por ID
        /// </summary>
        /// <param name="id_Tribunal"></param>
        /// <returns></returns>
        [HttpGet("ObtenerTribunales/{id_Tribunal}")]
        public async Task<IActionResult> GetPorId(int id_Tribunal)
        {
            using var connection = new SqlConnection(_cadenaSQL);
            var tribunal = await connection.QueryFirstOrDefaultAsync<Tribunales>(
                "SELECT * FROM Tribunales WHERE Id_Tribunal = @id_Tribunal", new { id_Tribunal });

            if (tribunal == null)
                return NotFound(new { mensaje = "Tribunal no encontrado" });

            return Ok(tribunal);
        }

       /// <summary>
       /// Crear tribunal
       /// </summary>
       /// <param name="dto"></param>
       /// <returns></returns>
        [HttpPost("CrearTribunal")]
        public async Task<IActionResult> Crear([FromBody] Tribunales dto)
        {
            const string sql = @"
            INSERT INTO Tribunales 
            (Nombre_Tribunal, Descripcion, Telefono, Estatus, Direccion, Distrito, MapsUrl)
            VALUES (@Nombre_Tribunal, @Descripcion, @Telefono, @Estatus, @Direccion, @Distrito, @MapsUrl);
            SELECT CAST(SCOPE_IDENTITY() as int);";

            using var connection = new SqlConnection(_cadenaSQL);
            var id = await connection.ExecuteScalarAsync<int>(sql, dto);
            return Ok(new { mensaje = "Tribunal creado correctamente", id });
        }

        /// <summary>
        ///  Actualizar tribunal
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("ActualizarTribunal/{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Tribunales dto)
        {
            const string sql = @"
            UPDATE Tribunales SET
                Nombre_Tribunal = @Nombre_Tribunal,
                Descripcion = @Descripcion,
                Telefono = @Telefono,
                Estatus = @Estatus,
                Direccion = @Direccion,
                Distrito = @Distrito,
                MapsUrl = @MapsUrl
            WHERE Id_Tribunal = @Id_Tribunal";

            dto.Id_Tribunal = id;

            using var connection = new SqlConnection(_cadenaSQL);
            var filas = await connection.ExecuteAsync(sql, dto);

            if (filas == 0)
                return NotFound(new { mensaje = "Tribunal no encontrado para actualizar" });

            return Ok(new { mensaje = "Tribunal actualizado correctamente" });
        }
        /// <summary>
        /// Eliminar un tribunal (en cuestion de las salas ya tuvo que haber pasado esa audiencia)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("EliminarTribunal/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            using var connection = new SqlConnection(_cadenaSQL);
            var filas = await connection.ExecuteAsync("DELETE FROM Tribunales WHERE Id_Tribunal = @id", new { id });

            if (filas == 0)
                return NotFound(new { mensaje = "Tribunal no encontrado para eliminar" });

            return Ok(new { mensaje = "Tribunal eliminado correctamente" });
        }
  
        /// <summary>
        /// Traer todas las salas
        /// </summary>
        /// <returns></returns>

        [HttpGet("Salas")]
        public async Task<IActionResult> GetTodas()
        {
            using var connection = new SqlConnection(_cadenaSQL);

            var query = @"
        SELECT 
            s.IdSala,
            s.Nombre,
            s.IdTribunal,
            t.Nombre_Tribunal
        FROM Salas s
        INNER JOIN Tribunales t ON s.IdTribunal = t.Id_Tribunal
    ";

            var salas = await connection.QueryAsync<SalaConTribunalDto>(query);
            return Ok(salas);
        }


        /// <summary>
        /// Obtener sala por ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Salaspor/{id}")]
        public async Task<IActionResult> PorId(int id)
        {
            using var connection = new SqlConnection(_cadenaSQL);

            var query = @"
        SELECT 
            s.IdSala,
            s.Nombre,
            s.IdTribunal,
            t.Nombre_Tribunal
        FROM Salas s
        INNER JOIN Tribunales t ON s.IdTribunal = t.Id_Tribunal
        WHERE s.IdSala = @id
    ";

            var sala = await connection.QueryFirstOrDefaultAsync<SalaConTribunalDto>(query, new { id });

            if (sala == null)
                return NotFound(new { mensaje = "Sala no encontrada" });

            return Ok(sala);
        }


        /// <summary>
        /// Crear nueva sala
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("CrearSalas")]
        public async Task<IActionResult> Crear([FromBody] SalaDto dto)
        {
            const string sql = @"
            INSERT INTO Salas (Nombre, IdTribunal)
            VALUES (@Nombre, @IdTribunal);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = new SqlConnection(_cadenaSQL);
            var id = await connection.ExecuteScalarAsync<int>(sql, dto);

            return Ok(new { mensaje = "Sala creada correctamente", id });
        }

        /// <summary>
        /// Actualizar sala
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("Actualizarsalas/{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] SalaDto dto)
        {
            dto.IdSala = id;

            const string sql = @"
            UPDATE Salas SET
                Nombre = @Nombre,
                IdTribunal = @IdTribunal
            WHERE IdSala = @IdSala;";

            using var connection = new SqlConnection(_cadenaSQL);
            var filas = await connection.ExecuteAsync(sql, dto);

            if (filas == 0)
                return NotFound(new { mensaje = "Sala no encontrada para actualizar" });

            return Ok(new { mensaje = "Sala actualizada correctamente" });
        }

        /// <summary>
        /// Eliminar sala
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("Eliminarsalas/{id}")]
        public async Task<IActionResult> Eliminarsalas(int id)
        {
            using var connection = new SqlConnection(_cadenaSQL);
            var filas = await connection.ExecuteAsync("DELETE FROM Salas WHERE IdSala = @id", new { id });

            if (filas == 0)
                return NotFound(new { mensaje = "Sala no encontrada para eliminar" });

            return Ok(new { mensaje = "Sala eliminada correctamente" });
        }

        /// <summary>
        /// Trae todas las salas que tiene un tribunal en especifico
        /// </summary>
        /// <param name="idTribunal"></param>
        /// <returns></returns>
        [HttpGet("{idTribunal}/Salas")]
        public async Task<IActionResult> ObtenerSalasPorTribunal(int idTribunal)
        {
            using var connection = new SqlConnection(_cadenaSQL);

            var query = @"SELECT IdSala, Nombre, IdTribunal
                  FROM Salas
                  WHERE IdTribunal = @idTribunal";

            var salas = await connection.QueryAsync<SalaDto>(query, new { idTribunal });

            if (!salas.Any())
                return NotFound(new { mensaje = "No se encontraron salas para el tribunal especificado." });

            return Ok(salas);
        }

    }
}








