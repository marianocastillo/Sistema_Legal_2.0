namespace Sistema_Legal_2._0.Server.Models
{
    public class LitigiosAsignadosAbogados
    {
        public int id_Ltg { get; set; }
        public string ltg_acto { get; set; }
        public DateTime? ltg_Fecha_Acto { get; set; }
        public DateTime? ltg_Fecha_Audiencia { get; set; }
        public string? Nombre_Tipo_Demanda { get; set; }
        public string? ltg_Demandante { get; set; }
        public string? ltg_Cedula_Demandante { get; set; }
        public string? desc_Sentencia { get; set; }
        public int? idUsuario { get; set; }
 
        public string? ltg_description { get; set; }
    }
}
