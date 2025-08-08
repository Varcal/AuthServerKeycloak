namespace AuthServerKeyCloak.Api.Models
{
    public class KeycloakToken
    {
        public string Access_Token { get; set; }
        public int Expire_in { get; set; }
        public string Refresh_token { get; set; }
        public int Refresh_expires_in { get; set; }
        public string Token_type { get; set; }
        public string Session_state { get; set; }
        public string Scope { get; set; }
    }
}
