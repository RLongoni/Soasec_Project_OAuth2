using Microsoft.Owin;
using Owin;
using System.IdentityModel.Claims;
using System.Web.Helpers;
using System.Web.Http;

[assembly: OwinStartupAttribute(typeof(Soasec_Oauth2_Project.Startup))]
namespace Soasec_Oauth2_Project
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // AntiForgeryConfig.UniqueClaimTypeIdentifier = "AccessToken";
            ConfigureAuth(app);
            var config = new HttpConfiguration();
            WebApiConfig.Register(config);
            app.UseWebApi(config);
            
        }
    }
}
