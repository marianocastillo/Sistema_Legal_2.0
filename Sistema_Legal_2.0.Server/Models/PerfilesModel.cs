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
    public IEnumerable<VistaAsignadaModel> Vistas { get; set; }
    public IEnumerable<UsuariosModel> Usuarios { get; set; }
    public int CantPermisos { get; set; }


}

public class PerfilDto
{
    public int IdPerfil { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public bool PorDefecto { get; set; }
    public int CantPermisos { get; set; }
    public List<int> Vistas { get; set; }
}


public class VistasYInicioResult
{
    public int IdVista { get; set; }

    public string Nombre { get; set; }

    public string Descripcion { get; set; }

    public string Url { get; set; }

    public bool Permiso { get; set; }

    public bool Principal { get; set; }

    public string IconClass { get; set; }

    public bool EsInicio { get; set; } // <-- Esta propiedad marca si esta vista es la ruta de inicio
}
