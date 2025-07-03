using Sistema_Legal_2._0.Server.Entities;

namespace Sistema_Legal_2._0.Server.Models
{
   public class LitigioConArchivo
{
    // Litigio
    public string ltg_acto { get; set; }
    public DateTime ltg_Fecha_Acto { get; set; }
    public string ltg_Cedula_Demandante { get; set; }
    public string ltg_Nombre_Demandante { get; set; }
    public string ltg_Tipo_Demandante { get; set; }
    public string ltg_Nacionalidad { get; set; }

    // Representante
    public string ltg_Cedula_Representante { get; set; }
    public string ltg_Nombre_Representante { get; set; }
    public string ltg_Nacionalidad_Representante { get; set; }

    // Otros IDs
    public int id_Tipo_Demanda { get; set; }
    public int? id_Sentencia { get; set; }
    public int id_usuario { get; set; }
    public int id_Estatus { get; set; }

    // Audiencia
    public DateTime? ltg_Fecha_Audiencia { get; set; }
    public int? SalaId { get; set; }
    public string? Tipo_audiencia { get; set; }


        // Evidencia
    public string NombreEvidencia { get; set; }
    public string comentario { get; set; }
    public IFormFile Archivo { get; set; }
}




}
