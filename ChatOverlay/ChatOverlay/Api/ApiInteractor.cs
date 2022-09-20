using MaJiCSoft.ChatOverlay.OAuth;
using MaJiCSoft.ChatOverlay.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaJiCSoft.ChatOverlay.Api
{
    public class ApiInteractor
    {
        private readonly ILogger logger;
        private readonly IOptions<TwitchClientOptions> clientOptions;
        private readonly OAuthState state;

        public ApiInteractor(ILogger<ApiInteractor> logger, IOptions<TwitchClientOptions> clientOptions, OAuthState state)
        {
            this.logger = logger;
            this.clientOptions = clientOptions;
            this.state = state;
        }

        public T Get<T>(string endpoint)
            where T : class, new()
        {
            var client = new RestClient(clientOptions.Value.ApiBase);
            var request = new RestRequest(endpoint);
            request.AddHeader("Authorization", $"Bearer {state.AccessToken}");
            var response = client.Get<T>(request);
            if (response.Data == null)
            {
                logger.LogWarning($"API Get {endpoint} failed: {response.Content}");
            }
            return response.Data;
        }
    }
}
