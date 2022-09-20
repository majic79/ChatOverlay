namespace MaJiCSoft.ChatOverlay.OAuth.Options
{
    public class OAuthServiceOptions
    {
        public string ClientId { get; set; }
        public string OAuthIssuer { get; set; }
        public string RedirectUri { get; set; }
        public string AuthorizeEndpoint { get; set; }
        public string ValidateEndpoint { get; set; }
        public string UserInfoEndpoint { get; set; }
    }
}
