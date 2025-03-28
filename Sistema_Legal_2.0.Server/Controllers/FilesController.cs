using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExcelDataReader;
using System.Data;
using System.Reflection;
using System.Numerics;
using Sistema_Legal_2._0.Server.Models;
using Sistema_Legal_2._0.Server.Repositories;
using Sistema_Legal_2._0.Server.Infraestructure;

//namespace Sistema_Legal_2._0.Server.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class FilesController : ControllerBase
//    {
//        private readonly IUserAccessor _userAccessor;
//        private readonly IConfiguration _configuration;
//        private readonly IWebHostEnvironment _hostingEnvironment;
//        private readonly Logger _logger;
//        private readonly db_silegContext _db_silegContext;
//        public FilesController(IConfiguration configuration, IUserAccessor userAccessor, IWebHostEnvironment hostingEnvironment, Logger logger, db_silegContext db_silegContext_)
//        {
//            _userAccessor = userAccessor;
//            _configuration = configuration;
//            _hostingEnvironment = hostingEnvironment;
//            _logger = logger;
//            _db_silegContext = db_silegContext_;
//        }
//        public class EjecucionGuardada
//        {
//            public DateTime FechaEjecucion { get; set; }
//            public List<EjecucionGasto>? Registros { get; set; }
//        }

//        [HttpPost()]
//        [Authorize]
//        public OperationResult Post([FromForm] IFormFile file)
//        {
//            try
//            {
//                if (file != null)
//                {
//                    List<EjecucionGasto> listado = new List<EjecucionGasto>();

//                    using (var reader = ExcelReaderFactory.CreateReader(file.OpenReadStream()))
//                    {
//                        var result = reader.AsDataSet();

//                        foreach (DataTable hoja in result.Tables)
//                        {
//                            DataColumnCollection columns = hoja.Columns;

//                            DataRowCollection rows = hoja.Rows;

//                            bool isFirstRow = true;
//                            List<string> ColumnNames = new List<string>();

//                            foreach (DataRow row in rows)
//                            {
//                                EjecucionGasto ejecucionGasto = new EjecucionGasto();
//                                for (int i = 0; i < columns.Count; i++)
//                                {
//                                    if (isFirstRow)
//                                    {
//                                        ColumnNames.Add(row[i].ToString().Trim());
//                                    }
//                                    else
//                                    {
//                                        PropertyInfo propertyInfo = ejecucionGasto.GetType().GetProperty(ColumnNames[i]);

//                                        if (propertyInfo == null)
//                                        {
//                                            return new OperationResult(false, "El archivo no cumple con el formato requerido. Utilizar la plantilla especificada.");
//                                        }

//                                        object parsedValue = Convert.ChangeType(row[i].ToString().Trim(), propertyInfo.PropertyType);
//                                        propertyInfo.SetValue(ejecucionGasto, parsedValue);
//                                    }
//                                }

//                                if (isFirstRow) isFirstRow = false;
//                                else listado.Add(ejecucionGasto);
//                            }
//                        }
//                    }

//                    return new OperationResult(true, file.FileName, listado);
//                }

//                return new OperationResult(false, "Debe seleccionar una archivo válido");
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex);
//                return new OperationResult(false, "Error al extraer archivo. Utilizar la plantilla especificada.");
//            }
//        }

        
    
//        //[Route("api/[controller]")]
//        //[ApiController]
//        //public class ArchivosController : ControllerBase
//        //{
//        //    [HttpGet("DescargarPlantilla")]
//        //    public IActionResult DescargarPlantilla()
//        //    {                
//        //        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Plantilla", "Plantilla Ejecucion del gasto.XLSX");

//        //        if (!System.IO.File.Exists(filePath))
//        //        {
//        //            return NotFound();
//        //        }

//        //        var bytes = System.IO.File.ReadAllBytes(filePath);
              
//        //        return File(bytes, "application/vnd.ms-excel", "Plantilla Ejecucion del gasto.XLSX");
//        //    }
//        //}
    

//    [HttpPost("Guardar", Name = "Guardar")]
//        [Authorize]
//        public OperationResult Guardar(EjecucionGuardada ejecucion)
//        {
//            using (var trx = _db_silegContext.Database.BeginTransaction())
//            {
//                try
//                {
//                    if (ejecucion.FechaEjecucion == null)
//                    {
//                        return new OperationResult(false, "Debe especificar la fecha de ejecución.");
//                    }

//                    if (ejecucion.Registros == null || ejecucion.Registros.Count == 0)
//                    {
//                        return new OperationResult(false, "No se han encontrado registros para guardar.");
//                    }

//                    var ExisteCarga = _db_silegContext.Set<Cargas>().Where(x => x.FechaEjecucionGasto == DateOnly.FromDateTime(ejecucion.FechaEjecucion)).Any();

//                    if (ExisteCarga)
//                    {
//                        return new OperationResult(false, "Ya existe una carga para la fecha especificada.");
//                    }

//                    _db_silegContext.Set<Cargas>().Add(new Cargas()
//                    {
//                        FechaCarga = DateTime.Now,
//                        idUsuario = _userAccessor.idUsuario,
//                        FechaEjecucionGasto = DateOnly.FromDateTime(ejecucion.FechaEjecucion)
//                    });

//                    _db_silegContext.SaveChanges();

//                    int idCarga = _db_silegContext.Set<Cargas>().OrderByDescending(x => x.idCarga).FirstOrDefault().idCarga;

//                    foreach(EjecucionGasto registro in ejecucion.Registros)
//                    {
//                        registro.idCarga = idCarga;
//                        _db_silegContext.Set<EjecucionGasto>().Add(registro);
//                    }

//                    _db_silegContext.SaveChanges();

//                    trx.Commit();

//                    return new OperationResult(true, "Se ha guardado exitosamente la ejecución.");
//                }
//                catch (Exception ex)
//                {
//                    trx.Rollback();
//                    _logger.LogError(ex);
//                    return new OperationResult(false, "Error al guardar ejecución.");
//                }
//            }

//        }
//    }
//}
