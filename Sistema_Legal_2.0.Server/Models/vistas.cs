﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Sistema_Legal_2._0.Server.Models;

public partial class vistas
{
    public int idVista { get; set; }

    public string nombre { get; set; }

    public string descripcion { get; set; }

    public string url { get; set; }

    public int idModulo { get; set; }

    public int idPadre { get; set; }
}