using Sistema_Legal_2._0.Server.Entities;

namespace Sistema_Legal_2._0.Server.Models
{
    public class SistemadeRegistro
    {


        public class LitigioCompletoDto
        {
            public Litigioses Litigio { get; set; }
            public List<int> AbogadosIds { get; set; }
            public List<AudienciaDetalleDto> Audiencias { get; set; }
            public List<CambioEstatusDto> CambiosEstatus { get; set; }
        }

        public class AudienciaDetalleDto
        {
            public Audiencias Audiencia { get; set; }
            public string NombreSala { get; set; }
            public string NombreTribunal { get; set; }
            public string DireccionTribunal { get; set; }
            public string MapsUrl { get; set; }
            public List<Evidencias> Evidencias { get; set; }
        }

        public class CambioEstatusDto
        {
            public int Id_historial { get; set; }
            public string Valor_anterior { get; set; }
            public string Valor_nuevo { get; set; }
            public DateTime Fecha_cambio { get; set; }
        }

        public partial class Litigioses
        {
            public int id_Ltg { get; set; }

            public string ltg_acto { get; set; }

            public DateTime ltg_Fecha_Acto { get; set; }

            public string ltg_Cedula_Demandante { get; set; }

            public string ltg_Nombre_Demandante { get; set; }

            public string ltg_Tipo_Demandante { get; set; }

            public string ltg_Nacionalidad { get; set; }

            public string ltg_Cedula_Representante { get; set; }

            public string ltg_Nombre_Representante { get; set; }

            public string ltg_Nacionalidad_Representante { get; set; }

            public int id_Tipo_Demanda { get; set; }

            public string Nombre { get; set; }

            public int id_Sentencia { get; set; }

            public string desc_Sentencia {get ; set; }
 
            public int id_usuario { get; set; }

            public int id_Estatus { get; set; }

            public string ltg_description { get; set; }
            public virtual ICollection<Usuarios> idUsuario { get; set; } = new List<Usuarios>();
        }
    }
}
