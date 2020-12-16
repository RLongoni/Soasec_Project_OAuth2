using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Services.Description;
using System.Windows.Forms;

namespace Soasec_Oauth2_Project.Utils
{
    public class TokenManager
    {
        private static string _secretKey = ConfigurationManager.AppSettings["SecretKey"];

        public static string GenerateToken(string userName)
        {
            byte[] key = Convert.FromBase64String(_secretKey);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims: new[] { new Claim(type: ClaimTypes.Name, value: userName) }),
                Expires = DateTime.UtcNow.AddMinutes(1), // da settare uno da 20 minuti
                SigningCredentials = new SigningCredentials(securityKey,
                    algorithm: SecurityAlgorithms.HmacSha256Signature)
            };
            
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            return handler.WriteToken(token);
        }
        public static string GenerateToken(string username, int expireMinutes)
        {
            var symmetricKey = Convert.FromBase64String(_secretKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, username)
                        }),

                Expires = now.AddMinutes(Convert.ToInt32(expireMinutes)),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            return token;
        }
        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {

                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken JWTToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (JWTToken == null)
                    return null;
                byte[] key = Convert.FromBase64String(_secretKey);
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)

                };
                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token,
                        parameters, out securityToken);
                return principal;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return null;
            }
        }
        public static ClaimsPrincipal OLDGetPrincipal(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return null;

                var symmetricKey = Convert.FromBase64String(_secretKey);

                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

                return principal;
            }

            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return null;
                
            }
        }
    }

}