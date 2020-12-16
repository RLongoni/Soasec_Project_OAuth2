using Microsoft.IdentityModel.Tokens;
using Soasec_Oauth2_Project.Filters;
using Soasec_Oauth2_Project.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Soasec_Oauth2_Project.Controllers
{
    public class JWTController : ApiController
    {

        [Route("JWT/GenerateJWT")]
        [HttpGet]
        [AllowAnonymous]
        // public string GetJWT(string username, string password)
        public HttpResponseMessage  GenerateJWT()
        {
            string result = TokenManager.GenerateToken("prova");
            var prova = TokenManager.GetPrincipal(result);
            /*Fargli creare il token e assegnarlo (come????)*/
            return Request.CreateResponse(HttpStatusCode.OK, result);
            
            // if (isValidUser(username, password))
            // { 
            //     return Request.CreateResponse(HttpStatusCode.OK, "Abbiamo Capito come usare il Token");
            // }
            // return Request.CreateResponse(HttpStatusCode.Unauthorized, "Abbiamo Fallito");
        }

        private bool isValidUser(string username, string password)
        { 
            // da controllare nel DB se esite l'utente 
            return false;
        }
        [HttpGet]
        [Authorize]
        public string HelloWorld()
        {
            return "HelloWorld";
        }
    }
}
