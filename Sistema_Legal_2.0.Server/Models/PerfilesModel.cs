using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sistema_Legal_2._0.Server.Models;

public class PerfilesModel
{
    [Key]
    public int idPerfil { get; set; }

    [Required(ErrorMessage = "El nombre del perfil es requerido")]
    [StringLength(50, ErrorMessage = "El nombre del perfil no puede exceder los 50 carácteres")]
    [Unicode(false)]
    public string Nombre { get; set; }

    [StringLength(100, ErrorMessage = "La descripción del perfil no puede exceder los 100 carácteres")]
    [Unicode(false)]
    public string? Descripcion { get; set; }
    public bool PorDefecto {  get; set; }

}