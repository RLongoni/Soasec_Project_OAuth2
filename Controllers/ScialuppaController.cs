using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using Soasec_Oauth2_Project.Utils;
using Soasec_Oauth2_Project.Models;
using Twilio.Rest.Trunking.V1;
using System.Windows.Forms;
using Soasec_Oauth2_Project.ResourceServer;
using System.Configuration;
using System.Net.Http;
using System.Web.Configuration;
using System.Threading.Tasks;
using System.Text;

namespace Soasec_Oauth2_Project.Controllers
{
    [RequireHttps]
    public class ScialuppaController : Controller
    {
        Authorizer OauthSoasec = new Authorizer();
        DataTable TabellaCatalogo = new DataTable();
        DBSpeaker _speaker = new DBSpeaker();
        // GET: AggiungiAllaScialuppa
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
        // Metodo che si invoca all'interno della pagina web catalogo
        [Authorize]
        public ActionResult AggiungiProdotto(ProdottoModel prodotto)
        {
            if (prodotto.QuantitaDisponibile > 0)
            {
                string updateQuantita = $"UPDATE [dbo].[Catalogo] SET [Quantità Disponibile]='{prodotto.QuantitaDisponibile - 1}' WHERE [Nome Prodotto] = '{prodotto.NomeProdotto}'";
                if (Session["cart"] == null)
                {
                    List<ProdottoModel> listaProdotti = new List<ProdottoModel>();

                    listaProdotti.Add(prodotto);
                    Session["cart"] = listaProdotti;
                    ViewBag.cart = listaProdotti.Count();
                    Session["count"] = 1;
                }
                else
                {
                    List<ProdottoModel> listaProdotti = (List<ProdottoModel>)Session["cart"];
                    listaProdotti.Add(prodotto);
                    Session["cart"] = listaProdotti;
                    ViewBag.cart = listaProdotti.Count();
                    Session["count"] = Convert.ToInt32(Session["count"]) + 1;
                }
                _speaker.DMLOpperation(updateQuantita);
                return RedirectToAction("Catalogo", "Home");
            }
            MessageBox.Show("Il prodotto non è disponibile");
            return RedirectToAction("Catalogo", "Home");
        }
        public async Task<ActionResult> LaMiaScialuppa()
        {
            bool emailFlag = ConfigurationManager.AppSettings["LoginEmail"].Equals("NoUsername");
            if (emailFlag || OauthSoasec.isAuthorized().Equals("Nuova Login"))
            {
                return RedirectToAction("Login", "Account");
            }
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
            return View((List<ProdottoModel>)Session["cart"]);
        }

        public ActionResult RimuoviProdotto(ProdottoModel prodotto)
        {
            DataTable disp = _speaker.GetQuantitaDisponibile(prodotto.NomeProdotto);
            int toUpdate = Convert.ToInt32(disp.Rows[0]["Quantità Disponibile"]);
            string updateQuantita = $"UPDATE [dbo].[Catalogo] SET [Quantità Disponibile]=' {toUpdate + 1}' WHERE [Nome Prodotto] = '{prodotto.NomeProdotto}'";
            List<ProdottoModel> lista = (List<ProdottoModel>)Session["cart"];
            for(int i = 0; i < lista.Count; i++)
            {
                if (lista[i].NomeProdotto.Equals(prodotto.NomeProdotto))
                {
                    lista.Remove(lista[i]);
                    break;
                }
            }
            _speaker.DMLOpperation(updateQuantita);
            Session["cart"] = lista;
            Session["count"] = Convert.ToInt32(Session["count"]) - 1;
            return RedirectToAction("LaMiaScialuppa", "Scialuppa");
        }


        public ActionResult PiazzaOrdine()
        {
            Session["cart"] = null;
            Session["count"] = 0;
            MessageBox.Show("Hai Completato L'acquisto. \n Verrai riportato alla Home");
            return RedirectToAction("Index", "Home");
        }
    }
}