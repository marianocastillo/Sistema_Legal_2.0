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
    public class FilesController : ControllerBase
    {

        private readonly string _filesServerPath;
        private readonly string _cadenaSQL;
        private IConfiguration configuration;
        private readonly IConfiguration _configuration;
        private readonly Logger _logger;
        private readonly db_silegContext _db_silegContext;
        public FilesController(IConfiguration config, db_silegContext db_SilegContext)
        {
            _db_silegContext = db_SilegContext;
            _filesServerPath = config.GetSection("Configuracion").GetSection("FilesServerPath").Value;
            _cadenaSQL = config.GetConnectionString("Sistema_Legal");
            _configuration = config;
        }







      


    }

}



