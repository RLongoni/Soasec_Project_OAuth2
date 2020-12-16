using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Soasec_Oauth2_Project.Models
{
    public sealed class BearerToken
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }

        [JsonProperty(PropertyName = "expires_in")]
        public int ExpiresInSeconds { get; set; }

        [JsonProperty(PropertyName = "userName")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = ".issued")]
        public DateTimeOffset? IssuedOn { get; set; }

        [JsonProperty(PropertyName = ".expires")]
        public DateTimeOffset? ExpiresOn { get; set; }
    }
}