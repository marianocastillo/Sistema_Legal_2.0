namespace Sistema_Legal_2._0.Server.Entities
{
    public class EvidenciaComentarioDTO
    {
        public IFormFile Archivo { get; set; }
        public string Comentario { get; set; }
        public int IdUsuario { get; set; }
        public int IdLitigio { get; set; }
    }

}
