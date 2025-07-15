using System;
using System.Collections.Generic;

namespace Sistema_Legal_2._0.Server.Models;

public class VistasModel
{
    public int idVista { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public string Url { get; set; }
    public int? idModulo { get; set; }
    public bool Permiso { get; set; }  // Indica si el perfil tiene acceso a esta vista
    public int? Principal { get; set; }  // ID del perfil para el que esta vista es principal
    public string IconClass { get; set; }  // Icono para menú (clase CSS)
    public int? Orden { get; set; }  // Para ordenar menús


}
public class VistaAsignadaModel
{
    public int idVista { get; set; }
    public bool Permiso { get; set; }
}

