using System.ComponentModel.DataAnnotations;

namespace ChatAppApi.Models
{
    public class SunucuUye
    {
        [Key]
        public int Id { get; set; }

        public int SunucuId { get; set; }
        public int KullaniciId { get; set; }

        // İleride "Moderatör", "Üye" gibi yetkileri de buradan yönetebiliriz
        public string Rol { get; set; } = "Üye";
    }
}