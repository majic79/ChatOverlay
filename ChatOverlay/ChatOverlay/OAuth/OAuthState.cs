using MaJiCSoft.ChatOverlay.OAuth.Options;
using MaJiCSoft.ChatOverlay.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Web;
using System.Windows.Input;

namespace MaJiCSoft.ChatOverlay.OAuth
{
    public class OAuthState : ViewModelBase
    {
        public OAuthRequest Request { get; set; }

        private string access_token;
        public string AccessToken
        {
            get => access_token;
            set => SetProperty(ref access_token, value);
        }

        private DateTime expiresOn;
        public DateTime ExpiresOn
        {
            get => expiresOn;
            set
            {

                if (SetProperty(ref expiresOn, value))
                {
                    OnPropertyChanged(nameof(IsSigned));
                    OnPropertyChanged(nameof(IsNotSigned));
                }
            }

        }

        public JwtSecurityToken IDToken { get; set; }

        public string[] Scopes { get; set; }

        private OAuthTokenValidation token;
        public OAuthTokenValidation Token
        {
            get => token;
            set => SetProperty(ref token, value);
        }

        public bool IsSigned => !string.IsNullOrEmpty(access_token) && expiresOn > DateTime.Now;
        public bool IsNotSigned => !IsSigned;


        private readonly ILogger logger;
        private readonly IOptions<OAuthServiceOptions> oauthServiceOptions;
        private readonly OAuthResponseValidator validator;

        public OAuthState(ILogger<OAuthState> logger, IOptions<OAuthServiceOptions> oauthServiceOptions, IWritableOptions<OAuthAccessOptions> oauthAccessOptions, OAuthResponseValidator validator)
        {
            this.logger = logger;
            this.oauthServiceOptions = oauthServiceOptions;
            this.validator = validator;
        }
        public string ValidateOAuthResponse(string response)
        {
            var dataUri = new Uri(response);
            var queryParams = HttpUtility.ParseQueryString(dataUri.Fragment.Substring(1));
            logger.LogInformation($"Decoded: {queryParams}");

            if (Request.WithPKCE)
            {
                // Check signature
                throw new NotImplementedException();
            }
            // Check State
            // Check Nonce
            if (Request.State.Equals(queryParams["state"]))
            {
                IDToken = validator.ValidateIdToken(queryParams["id_token"]);
                var idNonce = IDToken.Claims.Where(c => c.Type.Equals("nonce")).FirstOrDefault();
                if (!(idNonce == null && Request.Nonce == null))
                {
                    if (!Request.Nonce.Equals(idNonce.Value)) throw new Exception("Validation Error - Nonce mismatch");
                }

                Scopes = queryParams["scope"].Split(new char[] { ' ', '+' });
                Console.WriteLine("Token Validated");
                return queryParams["access_token"];
            }
            throw new Exception("Validation Error - State mismatch");
        }

        public UserInfo GetUserInfo()
        {
            var client = new RestClient(oauthServiceOptions.Value.OAuthIssuer);
            var request = new RestRequest(oauthServiceOptions.Value.UserInfoEndpoint);
            request.AddHeader("Authorization", $"Bearer {AccessToken}");
            var response = client.Get<UserInfo>(request);
            if (response.Data == null)
            {
                logger.LogWarning($"Issuer Get UserInfo failed: {response.Content}");
            }
            return response.Data;
        }
    }
}
