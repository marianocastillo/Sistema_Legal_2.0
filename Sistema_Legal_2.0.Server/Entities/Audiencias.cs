using Sistema_Legal_2._0.Server.Models;

namespace Sistema_Legal_2._0.Server.Entities
{
    class AudienciaDTO
    {
        public int Id_audiencia;
        public string TipoAudiencia { get; set; }
        public DateTime FechaAudiencia { get; set; }
        public string NumeroAudiencia { get; set; }
        public Tribunales Tribunal;
        public List<EvidenciaComentarioDetalleDTO> EvidenciasYComentarios { get; set; }
    }

}
