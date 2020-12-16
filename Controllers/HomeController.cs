using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using Soasec_Oauth2_Project.Utils;
using Soasec_Oauth2_Project.Models;
using Soasec_Oauth2_Project.ResourceServer;
using System.Net.Http;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Soasec_Oauth2_Project.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        Authorizer OauthSoasec = new Authorizer();
        DBSpeaker speaker = new DBSpeaker();
        DataTable TabellaCatalogo;
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Catalogo()
        {
            bool emailFlag = ConfigurationManager.AppSettings["LoginEmail"].Equals("NoUsername");
            if (emailFlag || OauthSoasec.isAuthorized().Equals("Nuova Login"))
            {
                return RedirectToAction("Login","Account");
            }
            else {
                if (OauthSoasec.isAuthorized().Equals("Nuova Token"))
                {
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage resultMessage = await client.PostAsync($"https://localhost:44391/token", 
                                                                                   new StringContent(string.Format("grant_type=password&username={0}&password={1}", 
                                                                                                                    ConfigurationManager.AppSettings["LoginEmail"], 
                                                                                                                    ConfigurationManager.AppSettings["LoginPassword"]), 
                                                                                                     Encoding.UTF8));
                        string newToken = resultMessage.Content.ReadAsStringAsync().Result;
                        var myConfiguration = WebConfigurationManager.OpenWebConfiguration("~");
                        myConfiguration.AppSettings.Settings["token"].Value = newToken;
                        myConfiguration.Save();
                    }
                }
                string selectCatalogo = "select * from [dbo].[Catalogo]";
                TabellaCatalogo = new DataTable();
                TabellaCatalogo = speaker.SelactAll(selectCatalogo);
                List<ProdottoModel> listaProdotti = new List<ProdottoModel>();
                for (int i = 0; i < TabellaCatalogo.Rows.Count; i++)
                {
                    ProdottoModel prodotto = new ProdottoModel();
                    prodotto.Id = Convert.ToInt32(TabellaCatalogo.Rows[i]["Id"]);
                    prodotto.NomeProdotto = TabellaCatalogo.Rows[i]["Nome Prodotto"].ToString();
                    prodotto.CategoriaProdotto = TabellaCatalogo.Rows[i]["Categoria Prodotto"].ToString();
                    prodotto.Descrizione = TabellaCatalogo.Rows[i]["Descrizione"].ToString();
                    prodotto.QuantitaDisponibile = Convert.ToInt32(TabellaCatalogo.Rows[i]["Quantità Disponibile"]);
                    prodotto.CostoInDobloni = Convert.ToInt32(TabellaCatalogo.Rows[i]["Costo in Dobloni"]);
                    prodotto.Immagine = TabellaCatalogo.Rows[i]["Immagine URL"].ToString();
                    listaProdotti.Add(prodotto);
                }
                return View(listaProdotti);
            }
        }
    }
}