namespace webapi.ViewModels
{
    public class CorreoVM
    {
        public string? subject { get; set; }
        public string? servicio { get; set; }
        public string? messageHtml { get; set; }
        public string[]? recipients { get; set; }
        public bool sendAnyWay { get; set; }


    }
}
