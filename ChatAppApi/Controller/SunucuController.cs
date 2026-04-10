using ChatAppApi.Data;
using ChatAppApi.DTOs;
using ChatAppApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatAppApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SunucuController : ControllerBase
    {
        public class SunucuGuncelleDto { public string YeniAd { get; set; } }

        private readonly AppDbContext _context;

        public SunucuController(AppDbContext context)
        {
            _context = context;
        }

        // GİRİŞ YAPAN KULLANICININ ÜYE OLDUĞU SUNUCULARI GETİRİR
        [HttpGet] // (Eğer burası [HttpGet("liste")] ise, WPF'teki URL'i /api/sunucu/liste yap)
        public async Task<IActionResult> GetKullaniciSunuculari()
        {
            // Token'dan giriş yapan kullanıcının ID'sini alıyoruz
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId)) return Unauthorized();

            // Sadece kullanıcının üye olduğu sunucuları getiriyoruz
            var sunucular = await (from s in _context.Sunucular
                                   join u in _context.SunucuUyeleri on s.Id equals u.SunucuId
                                   where u.KullaniciId == userId
                                   select new
                                   {
                                       s.Id,
                                       s.SunucuAdi
                                   }).ToListAsync();

            return Ok(sunucular);
        }
        [HttpPost("olustur")]
        public async Task<IActionResult> SunucuOlustur([FromBody] SunucuOlusturDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.SunucuAdi))
            {
                return BadRequest("Sunucu adı boş olamaz!");
            }

            // 🚀 YENİ: Token'ın içinden kullanıcının eşsiz ID'sini güvenle çekiyoruz
            var kurucuIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int kurucuId = int.TryParse(kurucuIdString, out int id) ? id : 0;

            if (kurucuId == 0)
            {
                return Unauthorized("Geçersiz kullanıcı kimliği.");
            }

            var yeniSunucu = new Sunucu
            {
                SunucuAdi = dto.SunucuAdi,
                KurucuId = kurucuId, // Artık int (Sayısal ID) tutuyoruz
                KurulumTarihi = DateTime.UtcNow
            };

            await _context.Sunucular.AddAsync(yeniSunucu);
            await _context.SaveChangesAsync();

            var varsayilanKanal = new Kanal
            {
                SunucuId = yeniSunucu.Id,
                KanalAdi = "genel"
            };
            var yeniUye = new SunucuUye
            {
                SunucuId = yeniSunucu.Id,
                KullaniciId = kurucuId,
                Rol = "Kurucu"
            };
            await _context.SunucuUyeleri.AddAsync(yeniUye);
            await _context.Kanallar.AddAsync(varsayilanKanal);
            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Sunucu başarıyla oluşturuldu!", sunucu = yeniSunucu });
        }

        // SUNUCU ADI GÜNCELLEME
        [HttpPut("guncelle/{id}")]
        public async Task<IActionResult> SunucuGuncelle(int id, [FromBody] SunucuGuncelleDto dto)
        {
            var sunucu = await _context.Sunucular.FindAsync(id);
            if (sunucu == null) return NotFound("Sunucu bulunamadı.");

            sunucu.SunucuAdi = dto.YeniAd;
            await _context.SaveChangesAsync();
            return Ok(new { mesaj = "Sunucu adı güncellendi." });
        }

        [HttpPost("katil/{sunucuId}")]
        public async Task<IActionResult> SunucuyaKatil([FromRoute] int sunucuId) // [FromRoute] ekledik
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId)) return Unauthorized();

            // 1. Sunucu var mı?
            var sunucu = await _context.Sunucular.FindAsync(sunucuId);
            if (sunucu == null) return NotFound("Sunucu bulunamadı."); // Hata burdan geliyorsa ID yanlıştır.

            // 2. Zaten üye mi?
            var varMi = await _context.SunucuUyeleri
                .AnyAsync(u => u.SunucuId == sunucuId && u.KullaniciId == userId);

            if (varMi) return BadRequest("Zaten bu sunucunun üyesisiniz.");

            // 3. Üyeliği ekle
            var yeniUye = new SunucuUye
            {
                SunucuId = sunucuId,
                KullaniciId = userId,
                Rol = "Üye"
            };

            await _context.SunucuUyeleri.AddAsync(yeniUye);
            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Başarıyla katıldınız!" });
        }

        [HttpDelete("sil/{id}")]
        public async Task<IActionResult> SunucuSil(int id)
        {
            var sunucu = await _context.Sunucular.FindAsync(id);
            if (sunucu == null) return NotFound("Sunucu bulunamadı.");

            // İlişkili tüm verileri (Kanallar, Roller, Üyeler) buluyoruz
            var kanallar = _context.Kanallar.Where(k => k.SunucuId == id);
            var roller = _context.Roller.Where(r => r.SunucuId == id);
            var uyeler = _context.SunucuUyeleri.Where(u => u.SunucuId == id);

            // Hepsini veritabanından topluca siliyoruz
            _context.Kanallar.RemoveRange(kanallar);
            _context.Roller.RemoveRange(roller);
            _context.SunucuUyeleri.RemoveRange(uyeler);
            _context.Sunucular.Remove(sunucu); // En son sunucunun kendisini siliyoruz

            await _context.SaveChangesAsync();
            return Ok(new { mesaj = "Sunucu ve bağlı tüm veriler yeryüzünden silindi!" });
        }
    }
}