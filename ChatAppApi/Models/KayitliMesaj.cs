using System.ComponentModel.DataAnnotations;

namespace ChatAppApi.Models
{
    public class KayitliMesaj
    {
        [Key]
        public int Id { get; set; }

        public string SunucuAdi { get; set; } = string.Empty;
        public string KanalAdi { get; set; } = string.Empty;
        public string Gonderen { get; set; } = string.Empty;
        public string Icerik { get; set; } = string.Empty;
        public DateTime Tarih { get; set; } = DateTime.Now;
    }
}