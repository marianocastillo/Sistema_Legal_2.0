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
    public bool Permiso { get; set; }
   public int idPerfil {get; set; }

}