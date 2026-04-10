using System.ComponentModel.DataAnnotations;

namespace ChatAppApi.Models
{
    public class Rol
    {
        [Key]
        public int Id { get; set; }

        public int SunucuId { get; set; } // Bu rol hangi sunucuya ait?

        public string RolAdi { get; set; } = string.Empty; // Örn: "Kurucu", "Moderatör"

        public string RenkHex { get; set; } = "#99AAB5"; // Discord varsayılan gri rol rengi
    }
}