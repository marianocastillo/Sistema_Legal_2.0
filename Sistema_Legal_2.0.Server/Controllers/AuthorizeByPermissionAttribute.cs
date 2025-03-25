
using Sistema_Legal_2._0.Server.Models.Enums;

namespace Sistema_Legal_2._0.Server.Controllers
{
    internal class AuthorizeByPermissionAttribute : Attribute
    {
        private PermisosEnum usuarios;
        private PermisosEnum editar_Usuario;

        public AuthorizeByPermissionAttribute(PermisosEnum usuarios, PermisosEnum editar_Usuario)
        {
            this.usuarios = usuarios;
            this.editar_Usuario = editar_Usuario;
        }
    }
}