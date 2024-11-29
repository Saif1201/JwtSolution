using System.Diagnostics;

namespace JwtExampleProject.ApiResponses
{
    public class LoginUserResponse
    {
        public string Token { get; set; } = string.Empty;
        public bool Flag { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
