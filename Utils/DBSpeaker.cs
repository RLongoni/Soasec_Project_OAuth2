using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace Soasec_Oauth2_Project.Utils
{
    public class DBSpeaker
    {
        SqlCommand SQLCommand;
        SqlDataAdapter SoasecAdapter;
        DataSet Soasec_Oauth2_DB;

        public static SqlConnection connectToDB()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
            else
            {
                connection.Open();
            }
            return connection;
        }

        public bool DMLOpperation(string SQLquery)
        {
            SQLCommand = new SqlCommand(SQLquery, DBSpeaker.connectToDB());
            int x = SQLCommand.ExecuteNonQuery();
            if (x == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public DataTable SelactAll(string query)
        {
            SoasecAdapter = new SqlDataAdapter(query, DBSpeaker.connectToDB());
            DataTable Soasec_Oauth2_DB = new DataTable();
            SoasecAdapter.Fill(Soasec_Oauth2_DB);
            return Soasec_Oauth2_DB;
        }

        public DataTable GetQuantitaDisponibile(string nomeProdotto)
        {
            string query = $"SELECT [Quantità Disponibile] FROM [dbo].[Catalogo] WHERE [Nome Prodotto] = '{nomeProdotto}'";
            SoasecAdapter = new SqlDataAdapter(query, DBSpeaker.connectToDB());
            DataTable risultato = new DataTable();
            SoasecAdapter.Fill(risultato);
            return risultato;
        }
    }
}
