namespace Sistema_Legal_2._0.Server.Models
{
    public class Litigio
    {
        public string ltg_acto { get; set; }
        public DateTime ltg_Fecha_Acto { get; set; }
        public int id_Tipo_Demanda { get; set; }
        public string ltg_Cedula_Demandante { get; set; }
        public string ltg_Nacionalidad { get; set; }
        public string ltg_Demandante { get; set; }
        public string ltg_Tipo_Demandante { get; set; }
        public string ltg_Cedula_Representante { get; set; }
        public string ltg_Nombre_Representante { get; set; }
        public DateTime? ltg_Fecha_Audiencia { get; set; }
        public string ltg_Doc_Demandante { get; set; }
        public DateTime? ltg_Fecha_Actualizacion { get; set; }
        public int? id_Tribunal { get; set; }
        public int? id_Sentencia { get; set; }
        public int id_usuario { get; set; }
        public int? id_ruta { get; set; }
        public int id_Estatus { get; set; }
    }
}
