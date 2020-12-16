using Microsoft.Ajax.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Soasec_Oauth2_Project.ResourceServer
{
    public class TokenManager
    {
        public enum JwtHashAlgorithm
        {
            HS256,
            HS384,
            HS512,
        }
        private static Dictionary<JwtHashAlgorithm, Func<byte[], byte[], byte[]>> HashAlgorithms;
        static  TokenManager()
        {
            HashAlgorithms = new Dictionary<JwtHashAlgorithm, Func<byte[], byte[], byte[]>>
            {
                { JwtHashAlgorithm.HS256, (key, value) => { using (var sha = new HMACSHA256(key)) { return sha.ComputeHash(value); } } },
                { JwtHashAlgorithm.HS384, (key, value) => { using (var sha = new HMACSHA384(key)) { return sha.ComputeHash(value); } } },
                { JwtHashAlgorithm.HS512, (key, value) => { using (var sha = new HMACSHA512(key)) { return sha.ComputeHash(value); } } }
            };
        }

        /*
         * 
         * ritorna Ok se il token è valido
         * ritorna Nuovo Token se è da refreshare il token con uno nuovo
         * ritorna Nuova Login se anche il refresh token è scaduto
         *
         */
        public string ControllToken(string token)
        {
            string validity = isValid(token);
            if (validity.Equals("Token Scaduto"))
            {
                string refValidity = isValid(getRefreshToken(token));
                if (refValidity.Equals("Token Valido"))
                    return "Nuovo Token"; // andrà generato un nuovo token
            }
            else if(validity.Equals("Token Valido"))
                return "Ok"; // si fa proseguire l'utente 
            return "Nuova Login"; // si reindirizza l'utente alla login page per farsi autenticare nuovamente
        }
        private string getRefreshToken(string token)
        {
            string payload = Decode(token, ConfigurationManager.AppSettings["SecretKey"],true);
            JObject payloadData = JObject.Parse(payload);
            return (string)payloadData["RefreshToken"];
        }
        private string isValid(string token)
        {
            string toBeControlled = Decode(token, ConfigurationManager.AppSettings["SecretKey"], true);
            string payload = string.Empty;
            if (toBeControlled.Equals("Firma Non Valida!"))
            {
                payload = Decode(token, ConfigurationManager.AppSettings["RefreshSecretKey"], true);
            }
            else
            {
                payload = toBeControlled;
            }
                             
            if (!payload.IsNullOrWhiteSpace())
            {
                JObject payloadData = JObject.Parse(payload);
                DateTime ExpiresDate = (DateTime)payloadData["ExpiresDate"];
                int result = DateTime.Compare(DateTime.Now, ExpiresDate);
                if (result <= 0)
                {
                    return "Token Valido";
                }
                else
                {
                    return "Token Scaduto";
                }
            }
            return "Token Non Valido";
        }
        public string Decode(string token, string key, bool verify)
        {
            var parts = token.Split('.');
            var header = parts[0];
            var payload = parts[1];
            byte[] crypto = Base64UrlDecode(parts[2]);

            var headerJson = Encoding.UTF8.GetString(Base64UrlDecode(header));
            var headerData = JObject.Parse(headerJson);
            var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));
            var payloadData = JObject.Parse(payloadJson);

            if (verify)
            {
                var bytesToSign = Encoding.UTF8.GetBytes(string.Concat(header, ".", payload));
                var keyBytes = Encoding.UTF8.GetBytes(key);
                // var algorithm = (string)headerData["alg"] == "HS256" ? "RS256" : (string)headerData["alg"];
                var algorithm = (string)headerData["alg"];
                var signature = HashAlgorithms[GetHashAlgorithm(algorithm)](keyBytes, bytesToSign);
                var decodedCrypto = Convert.ToBase64String(crypto);
                var decodedSignature = Convert.ToBase64String(signature);

                if (decodedCrypto != decodedSignature)
                {
                    return $"Firma Non Valida!";
                    // throw new ApplicationException(string.Format("Invalid signature. Expected {0} got {1}", decodedCrypto, decodedSignature));
                }
            }

            return payloadData.ToString();
        }

        /**/
        private static JwtHashAlgorithm GetHashAlgorithm(string algorithm)
        {
            switch (algorithm)
            {
                // case "RS256": return JwtHashAlgorithm.RS256;
                case "HS256": return JwtHashAlgorithm.HS256;
                case "HS384": return JwtHashAlgorithm.HS384;
                case "HS512": return JwtHashAlgorithm.HS512;
                default: throw new InvalidOperationException("Algorithm not supported.");
            }
        }

        // from JWT spec
        private static string Base64UrlEncode(byte[] input)
        {
            var output = Convert.ToBase64String(input);
            output = output.Split('=')[0]; // Remove any trailing '='s
            output = output.Replace('+', '-'); // 62nd char of encoding
            output = output.Replace('/', '_'); // 63rd char of encoding
            return output;
        }

        // from JWT spec
        private static byte[] Base64UrlDecode(string input)
        {
            var output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding
            switch (output.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: output += "=="; break; // Two pad chars
                case 3: output += "="; break; // One pad char
                default: throw new System.Exception("Illegal base64url string!");
            }
            var converted = Convert.FromBase64String(output); // Standard base64 decoder
            return converted;
        }
    }
}