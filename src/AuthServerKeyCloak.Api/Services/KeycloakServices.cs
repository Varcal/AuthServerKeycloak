using AuthServerKeyCloak.Api.Models;
using System.Runtime;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace AuthServerKeyCloak.Api.Services
{
    public interface IKeycloakServices
    {
        Task<LoginResponse> GetTokenAsync(LoginModel loginModel);
    }

    public class KeycloakServices : IKeycloakServices
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public KeycloakServices(HttpClient httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<LoginResponse> GetTokenAsync(LoginModel loginModel)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", _configuration.GetValue<string>("Keycloak:ClientId")),
                new KeyValuePair<string, string>("client_secret", _configuration.GetValue<string>("Keycloak:ClientSecret")),
                new KeyValuePair<string, string>("username", loginModel.Username),
                new KeyValuePair<string, string>("password", loginModel.Password),
                new KeyValuePair<string, string>("grant_type", "password")
            });


            var endpoint = _configuration.GetValue<string>("Keycloak:Endpoints:Token");
            var response = await _httpClient.PostAsync(endpoint, content);

            response.EnsureSuccessStatusCode();

            var keycloakToken = await response.Content.ReadFromJsonAsync<KeycloakToken>();
            return new LoginResponse(keycloakToken);
        }
    }
}
