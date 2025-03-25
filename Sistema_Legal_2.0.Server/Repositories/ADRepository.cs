
using Sistema_Legal_2._0.Server.Models;

using System.DirectoryServices.AccountManagement;
using DirectoryEntry = System.DirectoryServices.DirectoryEntry;

namespace Sistema_Legal_2.Server.Repositories
{
    public class ADRepository
    {
        public AdUser GetUserData(string userName)
        {
            AdUser user;
            using (var domainContext = new PrincipalContext(ContextType.Domain, "contraloria.com", "DC=contraloria,DC=com"))
            {
                using (var foundUser = UserPrincipal.FindByIdentity(domainContext, IdentityType.SamAccountName, userName))
                {
                    if (foundUser != null)
                    {
                        try
                        {
                            DirectoryEntry de = foundUser.GetUnderlyingObject() as DirectoryEntry;

                            user = new AdUser
                            {
                                firstName = foundUser.GivenName,
                                lastName = foundUser.Surname,
                                UserName = foundUser.UserPrincipalName,
                                EMail = foundUser.EmailAddress,
                                DisplayName = foundUser.DisplayName,
                                Cargo = (de.Properties["title"]?.Value ?? "").ToString(),
                                Cedula = (de.Properties["postalCode"]?.Value ?? "").ToString(),
                                Departamento = (de.Properties["department"]?.Value ?? "").ToString(),
                                Ubicacion = (de.Properties["physicalDeliveryOfficeName"]?.Value ?? "").ToString(),
                                ThumbnailPhoto = de.Properties["thumbnailPhoto"].Value as byte[]

                            };
                            return user;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
    }


}
