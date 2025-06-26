using Sistema_Legal_2._0.Server.Models;

namespace Sistema_Legal_2._0.Server.Entities
{
    public class EvidenciaDto
    {
        public int Id_Evidencia { get; set; }
        public string nombreEvidencia { get; set; }
        public string textoComentario { get; set; }
        public DateTime fechaComentario { get; set; }
        public string nombreArchivo { get; set; }
        public string rutaArchivo { get; set; }
    }

    public class AudienciaDto
    {
        public int Id_audiencia { get; set; }
        public string numeroAudiencia { get; set; }
        public string tipoAudiencia { get; set; }
        public DateTime fechaAudiencia { get; set; }
        public int Id_tribunal { get; set; }
        public List<EvidenciaDto> evidenciasYComentarios { get; set; }
    }
    public class TribunalDt
    {
        public int id_Tribunal { get; set; }
        public string nombre_Tribunal { get; set; }
        public string tribunal_Telefono { get; set; }
        public string tribunal_Direccion { get; set; }
        public string tribunal_Descripcion { get; set; }
        public DateTime ltg_Fecha_Audiencia { get; set; }
    }
    public class LitigioDetalleDto
    {
        public List<AudienciaDto> Audiencias { get; set; } = new();
        public TribunalDt TribunalFinal { get; set; }
    }


}
