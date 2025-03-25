using Microsoft.EntityFrameworkCore;
using Sistema_Legal_2.Server.Models;
using RegistroVisitas.Server.Repositories;
using Sistema_Legal_2._0.Server.Models;



namespace Sistema_Legal_2.Server.Repositories
{
    public class UsuariosRepo : Repository<Usuarios, UsuariosModel>
    {
        public UsuariosRepo(DbContext dbContext) : base
        (
            dbContext,
            new ObjectsMapper<UsuariosModel, Usuarios>(u => new Usuarios()
            {
                idUsuario = u.idUsuario,
                Usuario = u.NombreUsuario,
                Nombres = u.Nombres,
                Apellidos = u.Apellidos,                
                Activo = u.Activo,
                FechaCrea = u.FechaCrea,
                idPerfil = u.idPerfil
            }),
        (DB, filter) =>
        {
                return (from u in DB.Set<Usuarios>().Where(filter)
                        join p in DB.Set<Perfiles>() on u.idPerfil equals p.idPerfil
                        select new UsuariosModel()
                        {
                            idUsuario = u.idUsuario,
                            NombreUsuario = u.Usuario,
                            NombrePerfil = (string)(p.Nombre ?? ""),
                            Nombres = u.Nombres,
                            Apellidos = u.Apellidos,                            
                            FechaCrea = u.FechaCrea,
                            Activo = u.Activo,
                            idPerfil = (int)u.idPerfil
                        });
            }
        )
        {

        }

        public UsuariosModel GetByUsername(string nombreUsuario)
        {
            return this.Get(x => x.Usuario == nombreUsuario).FirstOrDefault();
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
}
