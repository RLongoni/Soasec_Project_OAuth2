
using Soasec_Oauth2_Project.ResourceServer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace Soasec_Oauth2_Project.Controllers
{
    [RequireHttps]
    public class TestController : ApiController
    {
        Authorizer OauthSoasec = new Authorizer();
        
        // [HttpGet]
        public async Task<string> HelloWorld()
        {
            if (OauthSoasec.isAuthorized().Equals("Ok"))
            {
                return "Token Valido: Hello World";
            }else if(OauthSoasec.isAuthorized().Equals("Nuovo Token"))
            { 
                using (HttpClient client = new HttpClient())
                {

                    HttpResponseMessage resultMessage = await client.PostAsync($"https://localhost:44391/token", new StringContent(string.Format("grant_type=password&username={0}&password={1}", ConfigurationManager.AppSettings["LoginEmail"], ConfigurationManager.AppSettings["LoginPassword"]), Encoding.UTF8));
                    // QUA da mettere l'update del token
                    string newToken = resultMessage.Content.ReadAsStringAsync().Result;
                }
                return $"Token Scaduto, un Nuovo Token Valido è stato generato per L'utente {ConfigurationManager.AppSettings["LoginEmail"]}: Hello World";
            }
            return "Token Non Valido. Occorre effettuare il Login";
        }
    }
}
