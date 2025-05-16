using System.DirectoryServices.AccountManagement;

namespace Sistema_Legal_2._0.Server.Providers
{
    public class ActiveDirectoryAuthenticationService
    {
        private readonly IConfiguration _configuration;

        public ActiveDirectoryAuthenticationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool ValidateCredentials2(string username, string password)
        {
            string connectionString = _configuration.GetConnectionString("ADConnectionString");

            using (PrincipalContext context = new PrincipalContext(ContextType.Domain, null, connectionString))
            {
                return context.ValidateCredentials(username, password);
            }
        }

        public bool ValidateCredentials(string username, string password)
        {
            string domainName = System.Environment.UserDomainName;
            string domainUserName = System.Environment.UserName;

            using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domainName, domainUserName, ContextOptions.SimpleBind.ToString()))
            {
                return context.ValidateCredentials(username, password);
            }
        }

        public UserPrincipal GetUser1(string username)
        {
            string? connectionString = _configuration.GetConnectionString("ADConnectionString");
            string domainName = System.Environment.UserDomainName;
            string domainUserName = System.Environment.UserName;

            using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domainName, domainUserName, ContextOptions.SimpleBind.ToString()))
            {
                UserPrincipal user = UserPrincipal.FindByIdentity(context, username);
                return user;
            }
        }

        public UserPrincipal GetUser(string username)
        {
            string domainName = System.Environment.UserDomainName;
            string domainUserName = System.Environment.UserName;
            using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domainName, domainUserName, ContextOptions.SimpleBind.ToString()))
            {
                UserPrincipal user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, username);
                return user;
            }
        }
    }

}
