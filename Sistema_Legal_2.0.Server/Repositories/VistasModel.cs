namespace Sistema_Legal_2.Server.Repositories
{
    public class VistasModel
    {
        public VistasModel()
        {
        }

        public object idVista { get; set; }
        public object Nombre { get; set; }
        public object Descripcion { get; set; }
        public object Url { get; set; }
        public bool Permiso { get; set; }
        public object idModulo { get; set; }
    }
}