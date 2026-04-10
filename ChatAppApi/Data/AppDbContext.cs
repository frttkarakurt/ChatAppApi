using ChatAppApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatAppApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<KayitliMesaj> Mesajlar { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Sunucu> Sunucular { get; set; }
        public DbSet<Kanal> Kanallar { get; set; }
        public DbSet<Rol> Roller { get; set; }
        public DbSet<SunucuUye> SunucuUyeleri { get; set; }
    }
}