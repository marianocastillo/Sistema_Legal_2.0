using Microsoft.EntityFrameworkCore;
using Sistema_Legal_2._0.Server.Entities;
using Sistema_Legal_2._0.Server.Models;
using Sistema_Legal_2._0.Server.Repositories;

public class PerfilesRepo : Repository<Perfiles, PerfilesModel>
{
    public PerfilesRepo(DbContext dbContext) : base
    (
        dbContext,
        new ObjectsMapper<PerfilesModel, Perfiles>(p => new Perfiles()
        {
            idPerfil = p.idPerfil,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            porDefecto = p.porDefecto ?? false
        }),
        (DB, filter) => (from p in DB.Set<Perfiles>().Where(filter)
                         select new PerfilesModel()
                         {
                             idPerfil = p.idPerfil,
                             Nombre = p.Nombre,
                             Descripcion = p.Descripcion,
                             porDefecto = p.porDefecto,
                             CantPermisos = DB.Set<perfilesVistas>().Count(a => a.idPerfil == p.idPerfil)
                         })
    )
    { }

    public  PerfilesModel Get(int Id)
    {
        return base.Get(p => p.idPerfil == Id).FirstOrDefault();
    }

    public IEnumerable<VistasModel> GetPermisos(int? idPerfil)
    {
        int id = idPerfil ?? 0;
        var permisosSet = dbContext.Set<perfilesVistas>().Where(p => p.idPerfil == id);

        return from v in dbContext.Set<vistas>()
               select new VistasModel()
               {
                   idVista = v.idVista,
                   Nombre = v.nombre,
                   Descripcion = v.descripcion,
                   Url = v.url,
                   Permiso = permisosSet.Any(p => p.idVista == v.idVista),
                   idModulo = v.idModulo,
                   Principal = v.Principal,
                   IconClass = v.Icono,
                   Orden = v.Orden
               };
    }

    public IEnumerable<UsuariosModel> GetUsuarios(int idPerfil)
    {
        UsuariosRepo usuariosRepo = new UsuariosRepo(dbContext);
        return usuariosRepo.Get(x => x.idPerfil == idPerfil);
    }

    public override Perfiles Add(PerfilesModel model)
    {
        using var trx = dbContext.Database.BeginTransaction();
        try
        {
            if (model.porDefecto ?? false)
            {
                dbContext.Set<Perfiles>().ToList().ForEach(p => p.porDefecto = false);
                SaveChanges();
            }
            Perfiles created = base.Add(model);

            if (model.Vistas != null)
            {
                var permisos = model.Vistas.Where(v => v.Permiso);
                dbContext.Set<perfilesVistas>().AddRange(permisos.Select(p => new perfilesVistas
                {
                    idPerfil = created.idPerfil,
                    idVista = p.idVista
                }));
                SaveChanges();
            }

            if (model.Usuarios != null)
            {
                var usuariosIds = model.Usuarios.Select(u => u.IdUsuario);
                var usuarios = dbContext.Set<Usuarios>().Where(u => usuariosIds.Contains(u.idUsuario)).ToList();
                usuarios.ForEach(u => u.idPerfil = created.idPerfil);
                SaveChanges();
            }

            trx.Commit();
            return created;
        }
        catch
        {
            trx.Rollback();
            throw;
        }
    }

    public override void Edit(PerfilesModel model)
    {
        using var trx = dbContext.Database.BeginTransaction();
        try
        {
            if (model.porDefecto ?? false)
            {
                dbContext.Set<Perfiles>().ToList().ForEach(p => p.porDefecto = false);
                SaveChanges();
            }

           
            var perfilExistente = dbContext.Set<Perfiles>().FirstOrDefault(p => p.idPerfil == model.idPerfil);
            if (perfilExistente != null)
            {
                perfilExistente.Nombre = model.Nombre;
                perfilExistente.Descripcion = model.Descripcion;
                perfilExistente.porDefecto = model.porDefecto ?? false;
                SaveChanges();
            }

            var permisosSet = dbContext.Set<perfilesVistas>();
            permisosSet.RemoveRange(permisosSet.Where(p => p.idPerfil == model.idPerfil));

            if (model.Vistas != null)
            {
                var newPermisos = model.Vistas.Where(v => v.Permiso);
                permisosSet.AddRange(newPermisos.Select(p => new perfilesVistas
                {
                    idPerfil = model.idPerfil,
                    idVista = p.idVista
                }));
                SaveChanges();
            }

        
            if (model.Usuarios != null)
            {
                var usuariosIds = model.Usuarios.Select(u => u.IdUsuario);
                var usuarios = dbContext.Set<Usuarios>().Where(u => usuariosIds.Contains(u.idUsuario)).ToList();
                usuarios.ForEach(u => u.idPerfil = model.idPerfil);
                SaveChanges();
            }

            trx.Commit();
        }
        catch
        {
            trx.Rollback();
            throw;
        }
    }


    public override void Delete(int id)
    {
        using var trx = dbContext.Database.BeginTransaction();
        try
        {
            base.Delete(id);

            var permisos = dbContext.Set<perfilesVistas>().Where(a => a.idPerfil == id);
            dbContext.RemoveRange(permisos);
            SaveChanges();

            trx.Commit();
        }
        catch
        {
            trx.Rollback();
            throw;
        }
    }

    public bool CanAccess(int idUsuario, int[] idVistas)
    {
        var query = from u in dbContext.Set<Usuarios>().Where(u => u.idUsuario == idUsuario && u.Activo)
                    join pv in dbContext.Set<perfilesVistas>().Where(p => idVistas.Contains(p.idVista)) on u.idPerfil equals pv.idPerfil
                    select pv;

        return query.Any();
    }

    public int GetPerfilDefault()
    {
        return base.Get(x => x.porDefecto == true).First().idPerfil;
    }

    public IEnumerable<VistasModel> GetVistas()
    {
        return from v in dbContext.Set<vistas>()
               select new VistasModel()
               {
                   idVista = v.idVista,
                   Nombre = v.nombre,
                   Descripcion = v.descripcion,
                   Url = v.url,
                   Principal = v.Principal,
                   IconClass = v.Icono,
                   idModulo = v.idModulo,
                   Orden = v.Orden
               };
    }

    public bool CanDelete(int id)
    {
        return !dbContext.Set<Usuarios>().Any(u => u.idPerfil == id);
    }
}
