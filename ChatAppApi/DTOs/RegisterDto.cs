namespace ChatAppApi.DTOs
{
    public class RegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty; // Kayıt olurken kullanıcı adı da isteyeceğiz
        public string Password { get; set; } = string.Empty;
    }
}
