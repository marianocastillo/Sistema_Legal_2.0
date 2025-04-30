using Microsoft.EntityFrameworkCore;
using Sistema_Legal_2._0.Server.Models;
using Sistema_Legal_2._0.Server.Entities;
namespace Sistema_Legal_2._0.Server.Repositories;
public class UsuariosRepo : Repository<Usuarios, UsuariosModel>
    {
        public UsuariosRepo(DbContext dbContext) : base
        (
            dbContext,
            new ObjectsMapper<UsuariosModel, Usuarios>(u => new Usuarios()
            {
                idUsuario = u.IdUsuario,
                nombreUsuario = u.NombreUsuario,
                nombres = u.Nombres,
           
                apellidos = u.Apellidos,                
                Activo = u.Activo,
                fechaCreacion = u.FechaCrea,
               
              
                idPerfil = u.IdPerfil
              
      
              
            }),
        (DB, filter) =>
        {
                return (from u in DB.Set<Usuarios>().Where(filter)
                        join p in DB.Set<Perfiles>() on u.idPerfil equals p.idPerfil
                        select new UsuariosModel()
                        {
                            IdUsuario = u.idUsuario,
                            NombreUsuario = u.nombreUsuario,          
                            Nombres = u.nombres,
                            Apellidos = u.apellidos,                            
                            FechaCrea = (DateTime)u.fechaCreacion,
                           
                            Activo = u.Activo,
                            IdPerfil = (int)u.idPerfil
                        });
            }
        )
        {

        }

        public UsuariosModel GetByUsername(string nombreUsuario)
        {
            return this.Get(x => x.nombreUsuario == nombreUsuario).FirstOrDefault();
        }

        public UsuariosModel Get(int id)
        {
            var result = base.Get(a => a.idUsuario == id).FirstOrDefault();

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

