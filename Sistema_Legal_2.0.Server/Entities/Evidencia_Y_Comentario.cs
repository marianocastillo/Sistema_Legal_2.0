namespace Sistema_Legal_2._0.Server.Entities
{
    public class EvidenciaComentarioDTO
    {
        public IFormFile Archivo { get; set; }
        public string Comentario { get; set; }
        public int IdUsuario { get; set; }
        public int IdLitigio { get; set; }
        public int IdAudiencia { get; set; }
        public String Nombre { get; set; }
    }

    public class EvidenciaComentarioDetalleDTO
    {
        public int ComentarioId { get; set; }
        public string TextoComentario { get; set; }
        public DateTime FechaComentario { get; set; }

        public string NombreArchivo { get; set; }
        public string RutaArchivo { get; set; }
        public DateTime FechaEvidencia { get; set; }
    }


}
