namespace Sistema_Legal_2._0.Server.Entities
{
    public class CrearAudienciaDto
    {
        public int IdLitigio { get; set; }
        public int IdTribunal { get; set; }
        public string Numero { get; set; }           
        public string Tipo { get; set; }         
        public DateTime Fecha { get; set; }
    }

}
