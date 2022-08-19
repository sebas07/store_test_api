using System.Text.Json.Serialization;

namespace API.Dtos
{
    public class UserDataDto
    {

        public string Username { get; set; }
        public string Email { get; set; }
        public ICollection<string> Roles { get; set; }
        public string Token { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Message { get; set; }
        [JsonIgnore]
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }

    }
}
