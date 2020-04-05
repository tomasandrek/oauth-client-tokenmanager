using System;

namespace Tomand.ClientTokenManager
{
    public class TokenInfo
    {
        public string access_token { get; set; }
        public string scope { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
        public DateTime expity_date {get; set; }
    }
}