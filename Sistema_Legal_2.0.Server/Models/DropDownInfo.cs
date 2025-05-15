namespace Sistema_Legal_2._0.Server.Models
{
    public class TribunalDto
    {
        public int Id_Tribunal { get; set; }
        public string Nombre_Tribunal { get; set; }
        public string Descripcion { get; set; }
        public string Telefono { get; set; }
        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }
        public int IdEstatus { get; set; }
        public string Direccion { get; set; }
    }

    public class TipoDemandaDto
    {
        public int id_demanda { get; set; }
        public string Nombre { get; set; }
        public int id_Estatus { get; set; }
    }

    public class EstatusLitigioDto
    {
        public int ltg_estatus { get; set; }
        public string ltg_description { get; set; }
    }


    public class DatosLitigioDto
    {
        public List<TribunalDto> Tribunales { get; set; }
        public List<TipoDemandaDto> TiposDemanda { get; set; }
        public List<EstatusLitigioDto> EstatusLitigios { get; set; }
    }

}
