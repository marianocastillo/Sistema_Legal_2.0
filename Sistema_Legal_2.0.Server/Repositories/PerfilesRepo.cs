using Sistema_Legal_2._0.Server.Models;
using Microsoft.EntityFrameworkCore;
<<<<<<< HEAD
using RegistroVisitas.Server.Repositories;
using System.Linq;
using Sistema_Legal_2._0.Server.Models;
using System;
using Sistema_Legal_2.Server.Repositories;
=======
using System.Reflection;
using Sistema_Legal_2._0.Server.Entities;
using Humanizer;
using Sistema_Legal_2._0.Server.Controller;
>>>>>>> Developer-Fronk

namespace Sistema_Legal_2._0.Server.Repositories
{

    public class PerfilesRepo : Repository<Perfiles, Models.PerfilesModel>
    {
        public PerfilesRepo(DbContext dbContext) : base
        (
            dbContext,
            new ObjectsMapper<Models.PerfilesModel, Perfiles>(p => new Perfiles()
            {
                Descripcion = p.Descripcion,
                idPerfil = p.idPerfil,
                Nombre = p.Nombre,
            }),
                    (DB, filter) => (from p in DB.Set<Perfiles>().Where(filter)
                                     select new Models.PerfilesModel()
                                     {
                                         Descripcion = p.Descripcion,
                                         idPerfil = p.idPerfil,
                                         Nombre = (string)p.Nombre,
                                     })
        )
        {

        }
        public Models.PerfilesModel Get(int Id)
        {
            var model = base.Get(p => p.idPerfil == Id).FirstOrDefault();
            return model;
        }


        public IEnumerable<VistasModel> GetPermisos(int? idPerfil)
        {
            int id = idPerfil ?? 0;

            var permisos = from p in dbContext.Set<perfilesVistas>()
                           join v in dbContext.Set<vistas>() on p.idVista equals v.idVista
                           where p.idPerfil == id
                           select new VistasModel()
                           {
                               idVista = v.idVista,
                               Nombre = v.nombre,
                               Descripcion = v.descripcion,
                               Url = v.url,
                               Permiso = true,
                               idModulo = v.idModulo
                           };

            return permisos.ToList();
        }

        public IEnumerable<UsuariosModel> GetUsuarios(int idPerfil)
        {
            UsuariosRepo usuariosRepo = new UsuariosRepo(dbContext);
            var listUsuarios = usuariosRepo.Get(x => x.idPerfil == idPerfil);
            return listUsuarios;
        }

        public bool CanAccess(int idUsuario, int[] idVistas)
        {
            var PVSet = from u in dbContext.Set<Usuarios>().Where(u => u.idUsuario == idUsuario && u.Activo == true)
                        join pv in dbContext.Set<perfilesVistas>().Where(a => idVistas.Contains(a.idVista)) on u.idPerfil equals pv.idPerfil
                        select pv;

            return PVSet.Any();
        }
        public int GetPerfilDefault()
        {
            var perfilDefault = base.Get(x => x.porDefecto == true).First();
            return perfilDefault.idPerfil;
        }



    }
}
