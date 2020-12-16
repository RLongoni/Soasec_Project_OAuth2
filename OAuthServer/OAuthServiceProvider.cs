using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.OAuth;
using System.Threading.Tasks;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Twilio.Rest.Events.V1.Sink;
using System.Security.Claims;
using System.Text;
using System.Security.Principal;
using Microsoft.Owin.Security;
using Soasec_Oauth2_Project.Models;

namespace Soasec_Oauth2_Project.OAuthServer
{
    public class OAuthServiceProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            //  Il client è sempre fidato
            /*
             * Si potrebbe controllare la validità del client attraverso una coppia di valori (string clientID, string clientSecret)
             * Pseudo codice
             * if(clientID == IDClientRegistrato AND clientSecret == SecretRegistrato)
             *  return OK; --> Client approvato
             * else 
             *  return Rejected; --> Client rigiutato 
             *  PS: IDClientRegistrato e SecretRegistrato sono registrati in una tabella del DB utilizzato da OAuth Server
             */
             context.Validated();
        }
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);
            if (user == null)
            {
                context.SetError("invalid_grant", "Username o Password non corrette.");
                return;
            }
            else
            {
                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, "JWT");

                var ticket = new AuthenticationTicket(oAuthIdentity, null);
                context.Validated(ticket);
            }
        }
    }
}