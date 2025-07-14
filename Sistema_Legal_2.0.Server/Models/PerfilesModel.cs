using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sistema_Legal_2._0.Server.Models;

public class PerfilesModel
{
    [Key]
    public int idPerfil { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public bool? porDefecto { get; set; }
    public IEnumerable<VistasModel> Vistas { get; set; }
    public IEnumerable<UsuariosModel> Usuarios { get; set; }
    public int CantPermisos { get; set; }


}