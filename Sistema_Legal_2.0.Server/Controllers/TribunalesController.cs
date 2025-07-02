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

        [HttpGet]
        public async Task<IActionResult> GetTodos()
        {
            using var connection = new SqlConnection(_cadenaSQL);
            var tribunales = await connection.QueryAsync<Tribunales>("SELECT * FROM Tribunales");
            return Ok(tribunales);
        }

        // 🔍 Obtener un tribunal por ID
        [HttpGet("ObtenerTribunales{id}")]
        public async Task<IActionResult> GetPorId(int id)
        {
            using var connection = new SqlConnection(_cadenaSQL);
            var tribunal = await connection.QueryFirstOrDefaultAsync<Tribunales>(
                "SELECT * FROM Tribunales WHERE Id_Tribunal = @id", new { id });

            if (tribunal == null)
                return NotFound(new { mensaje = "Tribunal no encontrado" });

            return Ok(tribunal);
        }

        // ➕ Crear tribunal
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

        // ✏️ Actualizar tribunal
        [HttpPut("ActualizarTribunal{id}")]
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

        // ❌ Eliminar tribunal
        [HttpDelete("EliminarTribunal{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            using var connection = new SqlConnection(_cadenaSQL);
            var filas = await connection.ExecuteAsync("DELETE FROM Tribunales WHERE Id_Tribunal = @id", new { id });

            if (filas == 0)
                return NotFound(new { mensaje = "Tribunal no encontrado para eliminar" });

            return Ok(new { mensaje = "Tribunal eliminado correctamente" });
        }
    }
}








