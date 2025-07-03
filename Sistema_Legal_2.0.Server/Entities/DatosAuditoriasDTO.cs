using Sistema_Legal_2._0.Server.Models;

namespace Sistema_Legal_2._0.Server.Entities
{
    public class EvidenciaDto
    {
        public int? Id_Evidencia { get; set; }
        public string? Nombre_Evidencia { get; set; }
        public string? Comentario_Evidencia { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Nombre_Archivo { get; set; }
        public string? Ruta_Archivo { get; set; }
    }
    public class SalasDto
    {
        public int? IdSala { get; set; }
        public string? Nombre { get; set; }
        public int? Id_tribunal { get; set; }
    }
    public class AudienciaDto
    {
        public int? Id_audiencia { get; set; }
        public string? numeroAudiencia { get; set; }
        public string? tipoAudiencia { get; set; }
        public DateTime? fechaAudiencia { get; set; }
        public int? IdSala { get; set; }
        public List<EvidenciaDto> evidenciasYComentarios { get; set; }
    }

    public class AudienciaFinalDto
    {
        public string? tipoAudiencia { get; set; }

        public int? IdSala { get; set; }
        public string? Nombre { get; set; }
        public int? id_Tribunal { get; set; }
        public string Distrito { get; set; }
        public string MapsUrl { get; set; }
        public string nombre_Tribunal { get; set; }
        public string tribunal_Telefono { get; set; }
        public string tribunal_Direccion { get; set; }
        public string tribunal_Descripcion { get; set; }
        public DateTime? ltg_Fecha_Audiencia { get; set; }

        public string? numeroAudiencia { get; set; }
    }

    public class TribunalDt
    {   
        public int? id_Tribunal { get; set; }
        public string nombre_Tribunal { get; set; }
        public string tribunal_Telefono { get; set; }
        public string tribunal_Direccion { get; set; }
        public string tribunal_Descripcion { get; set; }
        public DateTime? ltg_Fecha_Audiencia { get; set; }

    }
    public class LitigioDetalleDto
    { 
     
        public List<AudienciaDto> Audiencias { get; set; } = new();
        public AudienciaFinalDto TribunalFinal { get; set; }
    }


}
