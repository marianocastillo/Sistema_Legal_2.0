using Sistema_Legal_2._0.Server.Entities;

namespace Sistema_Legal_2._0.Server.Models
{
    public class LitigioConArchivo
    {
        public string ltg_acto { get; set; }

        public DateTime? ltg_Fecha_Acto { get; set; }

        public int id_Tipo_Demanda { get; set; }
        public string ltg_Demandante { get; set; }
        public string ltg_Cedula_Demandante { get; set; }
        public string ltg_Nacionalidad { get; set; }

        public string ltg_Tipo_Demandante { get; set; }
        public string ltg_Cedula_Representante { get; set; }
        public string ltg_Nombre_Representante { get; set; }
        public DateTime? ltg_Fecha_Audiencia { get; set; }
        public DateTime? ltg_Fecha_Actualizacion { get; set; }
        public int? id_Tribunal { get; set; }
        public int? id_Sentencia { get; set; }
        public int? id_usuario { get; set; }
        public int id_Estatus { get; set; }

        public string comentario { get; set; }


        public IFormFile Archivo { get; set; }
<<<<<<< HEAD
        public string NombreCarpeta { get; set; }
=======
>>>>>>> 3591f943cc82611f7e15ad8f5d338003275fafee


    }
}
