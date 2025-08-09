using AuthServerKeyCloak.Api.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AuthServerKeyCloak.Api.Services
{
    public interface IKeycloakServices
    {
        Task<LoginResponse> GetTokenAsync(LoginModel loginModel);
        Task CreateUserAsync(RegisterModel registerModel);
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

        public async Task CreateUserAsync(RegisterModel registerModel)
        {
            var adminLogin = new LoginModel
            {
                Username = _configuration.GetValue<string>("Keycloak:AdminUser:Username"),
                Password = _configuration.GetValue<string>("Keycloak:AdminUser:Password")
            };

            var adminToken = await GetTokenAsync(adminLogin);

            var userData = new
            {
                firstName = registerModel.FirstName,
                lastName = registerModel.LastName,
                email = registerModel.Email,
                username = registerModel.Username,
                enabled = true,
                credentials = new[]
                {
                    new { type = "password", value = registerModel.Password, temporary = false }
                }
            };

            var endpoint = _configuration.GetValue<string>("Keycloak:AdminUrl") + _configuration.GetValue<string>("Keycloak:Endpoints:Users");

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = JsonContent.Create(userData)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken.AccessToken);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}
