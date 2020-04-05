using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Tomand.ClientTokenManager
{
     public enum GrantType
    {
        [Description("client_credentials")]
        ClientCredentials,
        [Description("authorization_code")]
        AuthorizationCode,
        [Description("refresh_token")]
        RefreshToken
    }
    
    public class ClientSetting
    {
        public int UniqueId {get; set; }
        public Uri AuthServer {get; set; } 
        public GrantType GrantType { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Audience { get; set; }
        public string[] Scope { get; set; }
        public int MinimumTokenLifeTime { get; set; } = 20; // Default value is 20

        // Request parameters according to OAuth2 specification.
        public NameValueCollection AsFormCollection => new NameValueCollection()
        {
            { "grant_type", GrantType.ToDescriptionString() },
            { "client_id", ClientId },
            { "client_secret", ClientSecret },
            { "audience", Audience },
            { "scope", String.Join(" ", Scope) }
        };
    }
}