using System;
using System.ComponentModel.DataAnnotations;

namespace ChatAppApi.Models
{
    public class Sunucu
    {
        [Key]
        public int Id { get; set; }

        public string SunucuAdi { get; set; } = string.Empty;
        public int KurucuId { get; set; }
        public DateTime KurulumTarihi { get; set; } = DateTime.Now;
    }
}