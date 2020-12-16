using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Soasec_Oauth2_Project.Models
{
    public class ProdottoModel
    {
        public int Id { get; set; }
        public string NomeProdotto { get; set; }
        public string CategoriaProdotto { get; set; }
        public string Descrizione { get; set; }
        public int QuantitaDisponibile { get; set; }
        public int CostoInDobloni { get; set; }

        public string Immagine { get; set; }
    }
}