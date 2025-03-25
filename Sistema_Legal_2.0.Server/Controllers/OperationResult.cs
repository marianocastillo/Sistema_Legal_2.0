
using Sistema_Legal_2._0.Server.Models;

namespace Sistema_Legal_2.Server.Controllers
{
    public class OperationResult
    {
        private bool v1;
        private string v2;
        private Dictionary<string, string> errors;

        public OperationResult(bool v1, string v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }

        public OperationResult(bool v1, string v2, Models.Usuarios created)
        {
            this.v1 = v1;
            this.v2 = v2;
        }

        public OperationResult(bool v1, string v2, int idUsuario)
        {
            this.v1 = v1;
            this.v2 = v2;
        }

        public OperationResult(bool v1, string v2, Dictionary<string, string> errors)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.errors = errors;
        }

        public OperationResult(bool v1, string v2, UsuariosModel usuario) : this(v1, v2)
        {
        }
    }
}