using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Soasec_Oauth2_Project.ResourceServer
{
    public class Authorizer
    {
        TokenManager tokenController = new TokenManager();
        public string isAuthorized()
        {
            return tokenController.ControllToken(ConfigurationManager.AppSettings["token"]);
        }
    }
}