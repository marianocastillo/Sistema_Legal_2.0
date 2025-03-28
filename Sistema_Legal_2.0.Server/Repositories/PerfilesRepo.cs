using Sistema_Legal_2._0.Server.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;


namespace Sistema_Legal_2._0.Server.Repositories
{

    public class PerfilesRepo : Repository<Perfiles, PerfilesModel>
    {
        public PerfilesRepo(DbContext dbContext) : base
        (
            dbContext,
            new ObjectsMapper<PerfilesModel, Perfiles>(p => new Perfiles()
            {
                Descripcion = p.Descripcion,
                IdPerfil = p.idPerfil,
                Nombre = p.Nombre,
            }),
                    (DB, filter) => (from p in DB.Set<Perfiles>().Where(filter)
                                     select new PerfilesModel()
                                     {
                                         Descripcion = p.Descripcion,
                                         idPerfil = p.IdPerfil,
                                         Nombre = p.Nombre,
                                     })
        )
        {

        }
        public PerfilesModel Get(int Id)
        {
            var model = base.Get(p => p.IdPerfil == Id).FirstOrDefault();
            return model;
        }


        public IEnumerable<VistasModel> GetPermisos(int? idPerfil)
        {
            int id = idPerfil ?? 0;
            var permisosSet = dbContext.Set<PerfilesVistas>().Where(p => p.IdPerfil == id);
            return from v in dbContext.Set<VistasModel>()
                   select new VistasModel()
                   {
                       idVista = v.idVista,
                       Nombre = v.Nombre,
                       Descripcion = v.Descripcion,
                       Url = v.Url,
                       Permiso = permisosSet.Any(a => a.IdVista == v.idVista),                      
                       idModulo = v.idModulo,
                      
                   };
        }

        public IEnumerable<UsuariosModel> GetUsuarios(int idPerfil)
        {
            UsuariosRepo usuariosRepo = new UsuariosRepo(dbContext);
            var listUsuarios = usuariosRepo.Get(x => x.IdPerfil == idPerfil);
            return listUsuarios;
        }

        public bool CanAccess(int idUsuario, int[] idVistas)
        {
            var PVSet = from u in dbContext.Set<Usuarios>().Where(u => u.IdUsuario == idUsuario && u.Activo == true)
                        join pv in dbContext.Set<PerfilesVistas>().Where(a => idVistas.Contains(a.IdVista)) on u.IdPerfil equals pv.IdPerfil
                        select pv;

            return PVSet.Any();
        }
        public int GetPerfilDefault()
        {
            var perfilDefault = base.Get(x => x.PorDefecto == true).First();
            return perfilDefault.idPerfil;
        }



    }
}
