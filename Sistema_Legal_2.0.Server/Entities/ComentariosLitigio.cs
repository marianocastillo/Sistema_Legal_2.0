namespace Sistema_Legal_2._0.Server.Entities
{
    public class ComentariosLitigio
    {
        public int Id { get; set; }
        public int Id_Litigio { get; set; }
        public string Comentario { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public int Id_usuario { get; set; }
    }
}
