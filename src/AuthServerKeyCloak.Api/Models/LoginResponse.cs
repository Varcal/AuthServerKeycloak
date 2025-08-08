namespace AuthServerKeyCloak.Api.Models
{
    public class LoginResponse(KeycloakToken keycloakToken)
    {
        public string AccessToken { get; private set; } = keycloakToken.Access_Token;
        public int ExpireIn { get; private set; } = keycloakToken.Expire_in;
        public string RefreshToken { get; private set; } = keycloakToken.Refresh_token;
        public int RefreshExpiresIn { get; private set; } = keycloakToken.Refresh_expires_in;
        public string TokenType { get; private set; } = keycloakToken.Token_type;
        public string SessionState { get; private set; } = keycloakToken.Session_state;
        public string Scope { get; private set; } = keycloakToken.Scope;
    }
}