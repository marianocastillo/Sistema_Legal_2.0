using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sistema_Legal_2._0.Server.Models
{
    public class UsuariosConPerfilModel
    {
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "Se debe especificar el nombre de usuario")]
        [StringLength(100, ErrorMessage = "No puede exceder los 50 carácteres")]
        [Unicode(false)]
        public string NombreUsuario { get; set; }


        public int IdPerfil { get; set; }

        public int IdSupervisor { get; set; }

        [Required(ErrorMessage = "Se debe especificar los nombres")]
        [StringLength(100, ErrorMessage = "No puede exceder los 50 carácteres")]
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
}
