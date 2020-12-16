using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Soasec_Oauth2_Project.Provider
{
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private string clientID { get; set; }
        private string clientSecret { get; set; }

        public AuthorizationServerProvider(string client, string secret)
        {
            clientID = client;
            clientSecret = secret;
        }
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {

            if (clientID == ConfigurationManager.AppSettings["ClientIDSoasec"] &&
                clientSecret == ConfigurationManager.AppSettings["ClientSecretSoasec"]
                )
            {
                    context.Validated();
            }
            else
            {
                context.SetError("invalid_client", "Client credentials could not be retrieved from the Authorization header");
                context.Rejected();
            }
        }
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            UserManager<IdentityUser> userManager = context.OwinContext.GetUserManager<UserManager<IdentityUser>>();
            IdentityUser user;
            try
            {
                user = await userManager.FindAsync(context.UserName, context.Password);
            }
            catch
            {
                // Could not retrieve the user due to error.  
                context.SetError("server_error");
                context.Rejected();
                return;
            }
            if (user != null)
            {
                ClaimsIdentity identity = await userManager.CreateIdentityAsync(
                                                        user,
                                                        DefaultAuthenticationTypes.ExternalBearer);
                context.Validated(identity);
            }
            else
            {
                context.SetError("invalid_grant", "Invalid User Id or password'");
                context.Rejected();
            }
        }
    }
}