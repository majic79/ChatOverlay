using MaJiCSoft.ChatOverlay.OAuth.Options;
using System.Web;

namespace MaJiCSoft.ChatOverlay.OAuth
{

    public class OAuthRequest
    {
        private OAuthRequest()
        {
        }

        public string State { get; private set; }
        public bool WithPKCE { get; private set; }
        public string Nonce { get; private set; }
        public string CodeVerifier { get; private set; }
        public string AuthorizationRequestUri { get; private set; }
        public string RedirectUri { get; private set; }
        public string[] Scopes { get; private set; }

        public OAuthServiceOptions OAuthService { get; private set; }

        public static OAuthRequest BuildAuthorizeRequest(OAuthServiceOptions serviceOptions, OAuthResponseValidator validator, bool forceVerify, bool withPKCE, params string[] scopes)
        {
            var request = new OAuthRequest
            {
                State = Helpers.Base64UrlEncode(Helpers.RandomData(32)),
                Nonce = Helpers.Base64UrlEncode(Helpers.RandomData(32)),
                WithPKCE = withPKCE,
                CodeVerifier = Helpers.Base64UrlEncode(Helpers.RandomData(32)),
                Scopes = scopes,
                OAuthService = serviceOptions
            };

            var queryString = $"client_id={HttpUtility.UrlEncode(request.OAuthService.ClientId)}" +
                $"&redirect_uri={HttpUtility.UrlEncode(request.OAuthService.RedirectUri)}&" +
                $"&response_type=token+id_token&" +
                $"&scope={HttpUtility.UrlEncode(BuildScope(request.Scopes))}&" +
                $"&nonce={HttpUtility.UrlEncode(request.Nonce)}" +
                $"&state={HttpUtility.UrlEncode(request.State)}";

            if(forceVerify)
            {
                request.AuthorizationRequestUri += $"&force_verify=true";
            }

            if (request.WithPKCE)
            {
                // Generates state and PKCE values.
                string codeChallenge = Helpers.Base64UrlEncode(Helpers.Sha256(request.CodeVerifier));
                const string challengeMethod = "S256";
                request.AuthorizationRequestUri += $"&code_challenge={codeChallenge}&code_challenge_method={challengeMethod}";
            }

            request.AuthorizationRequestUri = $"{request.OAuthService.OAuthIssuer}/{request.OAuthService.AuthorizeEndpoint}?" + queryString;
            return request;
        }

        private static string BuildScope(string[] scopes)
        {
            var scope = string.Join(" ", scopes);
            return scope;
        }
    }
}
