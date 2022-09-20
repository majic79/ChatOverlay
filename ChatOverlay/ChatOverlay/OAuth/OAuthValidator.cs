using MaJiCSoft.ChatOverlay.OAuth.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MaJiCSoft.ChatOverlay.OAuth
{
    public class OAuthResponseValidator
    {

        private IOptions<OAuthServiceOptions> serviceOptions;
        private ILogger logger;

        public OAuthResponseValidator(ILogger<OAuthResponseValidator> logger, IOptions<OAuthServiceOptions> oauthServiceOptions)
        {
            this.serviceOptions = oauthServiceOptions;
            this.logger = logger;
        }

        public async Task<OAuthTokenValidation> ValidateToken(string access_token)
        {
            // Hit the Validate API endpoint
            var client = new RestClient(serviceOptions.Value.OAuthIssuer);
            var request = new RestRequest($"{serviceOptions.Value.ValidateEndpoint}");
            request.AddHeader("Authorization", $"OAuth {access_token}");
            var validate = await client.GetAsync<OAuthTokenValidation>(request);
            if(validate == null)
            {
                logger.LogWarning($"Validation failed!");
            }
            return validate;
        }

        public JwtSecurityToken ValidateIdToken(string idToken)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            if (!jwtHandler.CanReadToken(idToken)) throw new Exception("Token Invalid.");

            var token = jwtHandler.ReadJwtToken(idToken);
            if(token.Audiences.All(s => !s.Equals(serviceOptions.Value.ClientId))) throw new Exception("Audience not valid.");
            if(!token.Issuer.Equals(serviceOptions.Value.OAuthIssuer)) throw new Exception("Issuer not valid.");
            return token;
        }
    }
}
