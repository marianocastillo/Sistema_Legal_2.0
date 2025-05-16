namespace Sistema_Legal_2._0.Server.Models
{
    public class EvidenciasUploadModel
    {

        public int id_Ltg { get; set; }

        public string NombreCarpeta { get; set; }
        public int id_usuario { get; set; }
        public IFormFile Archivo { get; set; }
    }
}
