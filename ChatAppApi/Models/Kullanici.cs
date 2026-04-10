using System.ComponentModel.DataAnnotations;

namespace ChatAppApi.Models
{
    public class Kullanici
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime KayitTarihi { get; set; } = DateTime.UtcNow;
    }
}