using System;
using System.Configuration;
using System.Web.Http;
using System.Windows.Forms;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Twitter;
using Owin;
using Soasec_Oauth2_Project.Models;
using Soasec_Oauth2_Project.OAuthServer;
using Soasec_Oauth2_Project.ResourceServer;

namespace Soasec_Oauth2_Project
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {   
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
            HttpConfiguration httpConfig = new HttpConfiguration();

            ConfigureOAuthTokenGeneration(app);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
             {
                 AuthenticationType = "ApplicationCookie",
                 LoginPath = new PathString("/Account/Login"),
                  Provider = new CookieAuthenticationProvider()
                     {
                         OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                             validateInterval: TimeSpan.FromMinutes(1),
                             regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                     }
             });            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

          try
          {
              app.UseTwitterAuthentication(new TwitterAuthenticationOptions
              {
                  ConsumerKey = ConfigurationManager.AppSettings["API_Twitter_Key"],
                  ConsumerSecret = ConfigurationManager.AppSettings["API_Twitter_Secret_Key"],
                  BackchannelCertificateValidator = new CertificateSubjectKeyIdentifierValidator(new[]
                  {
                      "A5EF0B11CEC04103A34A659048B21CE0572D7D47", // VeriSign Class 3 Secure Server CA - G2
                      "0D445C165344C1827E1D20AB25F40163D8BE79A5", // VeriSign Class 3 Secure Server CA - G3
                      "7FD365A7C2DDECBBF03009F34339FA02AF333133", // VeriSign Class 3 Public Primary Certification Authority - G5
                      "39A55D933676616E73A761DFA16A7E59CDE66FAD", // Symantec Class 3 Secure Server CA - G4
                      "5168FF90AF0207753CCCD9656462A212B859723B", //DigiCert SHA2 High Assurance Server C‎A 
                      "B13EC36903F8BF4701D498261A0802EF63642BC3" //DigiCert High Assurance EV Root CA
                  })
              });
          
              app.UseFacebookAuthentication(
                 appId: ConfigurationManager.AppSettings["Facebook_ClientID"],
                 appSecret: ConfigurationManager.AppSettings["Facebook_ClientSecret"]);
          
              app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
              {
                  ClientId = ConfigurationManager.AppSettings["Google_ClientID"],
                  ClientSecret = ConfigurationManager.AppSettings["Google_ClientSecret"]
              });
          
          } catch (Exception ex)
          {
              MessageBox.Show(ex.Message);
          }
          
        }
        private void ConfigureOAuthTokenGeneration(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                
                AllowInsecureHttp = true, // da usare solo in fase di sviluppo
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(3),
                Provider = new OAuthServiceProvider(),
                AccessTokenFormat = new JWTFormat("OAuthServer"),
            };

            app.UseOAuthAuthorizationServer(OAuthServerOptions);
        }

    }
}