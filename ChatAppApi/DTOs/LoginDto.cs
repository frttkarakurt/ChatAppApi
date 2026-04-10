namespace ChatAppApi.DTOs
{
    public class LoginDto
    {
        public string Email { get; set; } = string.Empty; // Giriş yaparken sadece Email yeterli
        public string Password { get; set; } = string.Empty;
    }
}
