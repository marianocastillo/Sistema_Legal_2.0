namespace Sistema_Legal_2._0.Server.Models
{
    public class AdUser
    {
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string EMail { get; set; }
        public string Cedula { get; set; }
        public string Cargo { get; set; }
        public string Departamento { get; set; }
        public string Ubicacion { get; set; }
        public byte[] ThumbnailPhoto { get; set; }
    }
}
