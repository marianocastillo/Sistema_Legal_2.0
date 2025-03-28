using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sistema_Legal_2._0.Server.Models;

public class UsuariosModel
{
    public int IdUsuario { get; set; }


    [Required(ErrorMessage = "Se debe especificar el nombre de usuario")]
    [StringLength(50, ErrorMessage = "No puede exceder a los 50 carácteres")]
    [Unicode(false)]
    public string NombreUsuario { get; set; }
    public int IdPerfil { get; set; }
    public string? NombrePerfil { get; set; }

    //[Required(ErrorMessage = "Se debe especificar la cédula")]
    //[StringLength(13, ErrorMessage = "No puede exceder a los 13 carácteres")]
    //[Unicode(false)]
    //public string Cedula { get; set; }

    [Required(ErrorMessage = "Se debe especificar los nombres")]
    [StringLength(50, ErrorMessage = "No puede exceder a los 50 carácteres")]
    [Unicode(false)]
    public string Nombres { get; set; }

    [Required(ErrorMessage = "Se debe especificar los apellidos")]
    [StringLength(50, ErrorMessage = "No puede exceder a los 50 carácteres")]
    [Unicode(false)]
    public string Apellidos { get; set; }

    //[Required(ErrorMessage = "Se debe especificar el perfil")]
    //public int idPerfil { get; set; }
    //public string NombrePerfil { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCrea { get; set; }

    public bool Activo { get; set; }

}