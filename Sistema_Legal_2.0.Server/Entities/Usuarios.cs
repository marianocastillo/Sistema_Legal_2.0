﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Sistema_Legal_2._0.Server.Entities;

public partial class Usuarios
{
    public int idUsuario { get; set; }

<<<<<<< HEAD:Sistema_Legal_2.0.Server/Models/Usuarios.cs
    public int? idPerfil { get; set; }
=======
    public string nombres { get; set; }

    public string apellidos { get; set; }

    public DateTime? fechaCreacion { get; set; }
>>>>>>> Developer-Fronk:Sistema_Legal_2.0.Server/Entities/Usuarios.cs

    public string Usuario { get; set; }

    public DateTime FechaCrea { get; set; }

    public bool Activo { get; set; }

<<<<<<< HEAD:Sistema_Legal_2.0.Server/Models/Usuarios.cs
    public string Nombres { get; set; }

    public string Apellidos { get; set; }
=======
    public string nombreUsuario { get; set; }


    public int? idSupervisor { get; set; }

    public virtual ICollection<Litigios> Litigios { get; set; } = new List<Litigios>();

    public virtual Perfiles idPerfilNavigation { get; set; }

>>>>>>> Developer-Fronk:Sistema_Legal_2.0.Server/Entities/Usuarios.cs
}