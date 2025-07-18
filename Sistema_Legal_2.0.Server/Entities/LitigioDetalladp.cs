using Sistema_Legal_2._0.Server.Models;

namespace Sistema_Legal_2._0.Server.Entities
{
    public class LitigioDetallado
    {
        public int id_Ltg { get; set; }
        public string ltg_acto { get; set; }
        public DateTime? ltg_Fecha_Acto { get; set; }
     
       
        public int? TipoDemanda_Id { get; set; }
        public string? TipoDemanda_Nombre { get; set; }
        public int TipoDemanda_Estatus { get; set; }

        public string? ltg_Cedula_Demandante { get; set; }
        public string? ltg_Nombre_Demandante { get; set; }
        public string ltg_Tipo_Demandante { get; set; }

        public string ltg_Nacionalidad { get; set; }

        public string ltg_Nombre_Representante { get; set; }

        public string ltg_Nacionalidad_Representante { get; set; }
        public string ltg_Cedula_Representante { get; set; }


        public DateTime? ltg_Fecha_Audiencia { get; set; }

        public int? id_Sentencia { get; set; }
        public string? desc_Sentencia { get; set; }

  
        public int? ltg_estatus { get; set; }
        public string? Estatus_Descripcion { get; set; }
    }

    public class LitigioDetalladoS
    {
        public int id_Ltg { get; set; }
        public string ltg_acto { get; set; }
        public DateTime? ltg_Fecha_Acto { get; set; }
        public string? TipoDemanda_Nombre { get; set; }
        public string? ltg_Cedula_Demandante { get; set; }
        public string? ltg_Nombre_Demandante { get; set; }

        public DateTime? ltg_Fecha_Audiencia { get; set; }
        public string? desc_Sentencia { get; set; }
        public bool Asignado {  get; set; }
        public int? ltg_estatus { get; set; }
        public string? Estatus_Descripcion { get; set; }
    }

    public class LitigiosResponse
    {
        public int TotalAsignados { get; set; }
        public int TotalSinAsignar { get; set; }
        public List<LitigioDetalladoS> Asignados { get; set; } = new();
        public List<LitigioDetalladoS> SinAsignar { get; set; } = new();
    }

}
