using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthServerKeyCloak.Api.Models
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
