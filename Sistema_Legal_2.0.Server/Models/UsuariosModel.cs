using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sistema_Legal_2._0.Server.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

public class UsuariosModel
{
    public int IdUsuario { get; set; }

    [Required(ErrorMessage = "Se debe especificar el nombre de usuario")]
    [StringLength(50, ErrorMessage = "No puede exceder los 50 carácteres")]
    [Unicode(false)]
    public string NombreUsuario { get; set; }


    public int IdPerfil { get; set; }

    public int? IdSupervisor { get; set; }

    [Required(ErrorMessage = "Se debe especificar los nombres")]
    [StringLength(50, ErrorMessage = "No puede exceder los 50 carácteres")]
    [Unicode(false)]
    public string Nombres { get; set; }

    public string nombrePerfil { get; set; }

    [Required(ErrorMessage = "Se debe especificar los apellidos")]
    [StringLength(100, ErrorMessage = "No puede exceder los 100 carácteres")]
    [Unicode(false)]
    public string Apellidos { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public bool Activo { get; set; }

}
