using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Web;

namespace Soasec_Oauth2_Project.OAuthServer
{
    public class JWTFormat : ISecureDataFormat<AuthenticationTicket>
    {

        private readonly string _issuer = string.Empty;
        private static Dictionary<JwtHashAlgorithm, Func<byte[], byte[], byte[]>> HashAlgorithms;
        public enum JwtHashAlgorithm
        {
            HS256,
            HS384,
            HS512
        }


        public JWTFormat(string issuer)
        {
            _issuer = issuer;
            HashAlgorithms = new Dictionary<JwtHashAlgorithm, Func<byte[], byte[], byte[]>>
            {
                { JwtHashAlgorithm.HS256, (key, value) => { using (var sha = new HMACSHA256(key)) { return sha.ComputeHash(value); } } },
                { JwtHashAlgorithm.HS384, (key, value) => { using (var sha = new HMACSHA384(key)) { return sha.ComputeHash(value); } } },
                { JwtHashAlgorithm.HS512, (key, value) => { using (var sha = new HMACSHA512(key)) { return sha.ComputeHash(value); } } }
            };
        }

        /*Protect Genererà il JWT Token*/
        public string Protect(AuthenticationTicket data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            //string prova = GenerateToken(data.Identity.Name, _issuer);
            var refreshTokenPayload = new
            {
                unique_name = data.Identity.Name,
                Issuer = _issuer,
                IssuedAt = DateTime.Now,
                ExpiresDate = DateTime.Now.AddMinutes(20), // deve essere più longevo dell'Access Token
                Type = "Refresh Token"
            };
            var refreshToken = Encode(refreshTokenPayload, ConfigurationManager.AppSettings["RefreshSecretKey"], JwtHashAlgorithm.HS256);

            var payload = new
            {
                unique_name = data.Identity.Name,
                Audience = "SoasecUser",
                Issuer = _issuer,
                IssuedAt = DateTime.Now,
                ExpiresDate = DateTime.Now.AddMinutes(3),
                RefreshToken = refreshToken,
                Type = "Access Token"
            };
            string result = Encode(payload, ConfigurationManager.AppSettings["SecretKey"], JwtHashAlgorithm.HS256);
            return result;
        }

            public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
        public static string Encode(object payload, string key, JwtHashAlgorithm algorithm)
        {
            return Encode(payload, Encoding.UTF8.GetBytes(key), algorithm);
        }

        public static string Encode(object payload, byte[] keyBytes, JwtHashAlgorithm algorithm)
        {
            var segments = new List<string>();
            var header = new { alg = algorithm.ToString(), typ = "JWT" };

            byte[] headerBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(header, Formatting.None));
            byte[] payloadBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload, Formatting.None));

            segments.Add(Base64UrlEncode(headerBytes));
            segments.Add(Base64UrlEncode(payloadBytes));

            var stringToSign = string.Join(".", segments.ToArray());

            var bytesToSign = Encoding.UTF8.GetBytes(stringToSign);

            byte[] signature = HashAlgorithms[algorithm](keyBytes, bytesToSign);
            segments.Add(Base64UrlEncode(signature));

            return string.Join(".", segments.ToArray());
        }
        private static string Base64UrlEncode(byte[] input)
        {
            var output = Convert.ToBase64String(input);
            output = output.Split('=')[0]; // Remove any trailing '='s
            output = output.Replace('+', '-'); // 62nd char of encoding
            output = output.Replace('/', '_'); // 63rd char of encoding
            return output;
        }
    }
}