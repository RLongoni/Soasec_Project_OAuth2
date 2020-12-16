using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Ajax.Utilities;

namespace Soasec_Oauth2_Project.Controllers
{
    // TODO: CANCELLARE QUESTA CLASSE A PROGETTO FINITO 
    public class OLDServiceConnectorController : Controller
    {
        private static string WebAPIURL = "https://localhost:44319/";
        // GET: ServiceConnector
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> GetAuthentication(string email, string password)
        {
            /*Collegamento tra applicazione e webAPi*/
            var tokenBased = string.Empty;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri(WebAPIURL);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var responseMessage = await client.GetAsync("Account/ValidateLogin?username=admin&password=admin");
                if (responseMessage.IsSuccessStatusCode)
                {
                    var resultMessage = responseMessage.Content.ReadAsStringAsync().Result;
                    tokenBased = JsonConvert.DeserializeObject<string>(resultMessage);
                    Session["TokenNumber"] = tokenBased;
                    Session["Username"] = "admin";
                }
            }
            /*TODO: Rimetterlo in un metodo a parte, forse un controllore*/
            return Content(tokenBased);
        }
        public async Task<ActionResult> IsTokenValid(string Token)
        {
            string ReturnMessage = string.Empty;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri(WebAPIURL);
                var responseMessage = await client.GetAsync($"Account/GetEmployee?toValidateToken={Token}");
                if (responseMessage.IsSuccessStatusCode)
                {
                    var resultMessage = responseMessage.Content.ReadAsStringAsync().Result;
                    ReturnMessage = JsonConvert.DeserializeObject<string>(resultMessage);
                }
            }
            return Content(ReturnMessage);
        }
        //stringa di test1: 
        //stringa id test2: 
        public async Task<ActionResult> IsAValidUser(string username, string password)
        {
            string ReturnMessage = string.Empty;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri(WebAPIURL);
                var responseMessage = await client.GetAsync($"Account/ExistUsername?username={username}&password={password}");
                if (responseMessage.IsSuccessStatusCode)
                {
                    var resultMessage = responseMessage.Content.ReadAsStringAsync().Result;
                    ReturnMessage = JsonConvert.DeserializeObject<string>(resultMessage);
                }
            }
            
            return Content(ReturnMessage);
        }
    }


}