using System;
using System.ComponentModel.DataAnnotations;

namespace ChatAppApi.Models
{
    public class Kanal
    {
        [Key]
        public int Id { get; set; }

        public int SunucuId { get; set; } // Bu kanal hangi sunucuya ait? (İlişki)

        public string KanalAdi { get; set; } = string.Empty;
        public string Tip { get; set; } = "Metin";
    }
}