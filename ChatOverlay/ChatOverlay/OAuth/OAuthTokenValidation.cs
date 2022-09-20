using System.Collections.Generic;

namespace MaJiCSoft.ChatOverlay.OAuth
{
    public class OAuthTokenValidation
    {
        public string ClientId { get; set; }
        public string Login { get; set; }
        public IEnumerable<string> Scopes { get; set; }
        public string UserId { get; set; }
        public int ExpiresIn { get; set; }
    }
}