using Microsoft.EntityFrameworkCore;
using Sistema_Legal_2._0.Server.Models;

namespace Sistema_Legal_2._0.Server.Repositories;
public class UsuariosRepo : Repository<Usuarios, UsuariosModel>
    {
        public UsuariosRepo(DbContext dbContext) : base
        (
            dbContext,
            new ObjectsMapper<UsuariosModel, Usuarios>(u => new Usuarios()
            {
                IdUsuario = u.IdUsuario,
                Nombres = u.Nombres,
                Apellidos = u.Apellidos,                
                Activo = u.Activo,
                FechaCreacion = u.FechaCrea,
                IdPerfil = u.IdPerfil,
              
            }),
        (DB, filter) =>
        {
                return (from u in DB.Set<Usuarios>().Where(filter)
                        join p in DB.Set<Perfiles>() on u.IdPerfil equals p.IdPerfil
                        select new UsuariosModel()
                        {
                            IdUsuario = u.IdUsuario,
                            NombreUsuario = u.NombreUsuario,
                            NombrePerfil = p.Nombre ?? "",
                            Nombres = u.Nombres,
                            Apellidos = u.Apellidos,                            
                            FechaCrea = u.FechaCreacion,
                            Activo = u.Activo,
                            IdPerfil = (int)u.IdPerfil
                        });
            }
        )
        {

        }

        public UsuariosModel GetByUsername(string nombreUsuario)
        {
            return this.Get(x => x.Nombres == nombreUsuario).FirstOrDefault();
        }

        public UsuariosModel Get(int id)
        {
            var result = base.Get(a => a.IdUsuario == id).FirstOrDefault();

            if (result != null)
            {
                return result;
            }

            return null;
        }
         
        public AdUser? GetADUser(string UserName)
        {
            ADRepository adRepository = new ADRepository();

            return (adRepository.GetUserData(UserName.ToLower()));
        }
       
    }

