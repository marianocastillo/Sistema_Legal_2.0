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
        public string? ltg_Nacionalidad { get; set; }
        public string? ltg_Demandante { get; set; }
        public string? ltg_Tipo_Demandante { get; set; }

        public string? ltg_Cedula_Representante { get; set; }
        public string? ltg_Nombre_Representante { get; set; }

        public DateTime? ltg_Fecha_Audiencia { get; set; }
        public DateTime? ltg_Fecha_Actualizacion { get; set; }

        public int? Id_Tribunal { get; set; }
        public string? Nombre_Tribunal { get; set; }
        public string? Tribunal_Descripcion { get; set; }
        public string? Tribunal_Telefono { get; set; }
        public string? Tribunal_Direccion { get; set; }
        public int? Tribunal_Estatus { get; set; }

        public int? id_Sentencia { get; set; }
        public string? desc_Sentencia { get; set; }

        public int? idUsuario { get; set; }
        public string? NombreUsuario { get; set; }
        public int? idPerfil { get; set; }

        public int? id_Ruta { get; set; }
        public string? Ruta { get; set; }
        public string? Ruta_Nombre { get; set; }
        public int? Ruta_idUsuario { get; set; }

        public int? ltg_estatus { get; set; }
        public string? Estatus_Descripcion { get; set; }
    }
}
