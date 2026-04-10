using ChatAppApi.Data;
using ChatAppApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatAppApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class KanalController : ControllerBase
    {
        private readonly AppDbContext _context;

        public KanalController(AppDbContext context)
        {
            _context = context;
        }

        public class KanalIslemiDto
        {
            public int SunucuId { get; set; }
            public string KanalAdi { get; set; } = string.Empty;
            public string Tip { get; set; } = "Metin"; // 🚀 Yeni eklendi
        }

        [HttpPost("olustur")]
        public async Task<IActionResult> KanalOlustur([FromBody] KanalIslemiDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.KanalAdi)) return BadRequest("Kanal adı boş olamaz.");

            var yeniKanal = new Kanal
            {
                SunucuId = dto.SunucuId,
                KanalAdi = dto.KanalAdi.ToLower().Replace(" ", "-"),
                Tip = dto.Tip // 🚀 Tür kaydediliyor
            };

            await _context.Kanallar.AddAsync(yeniKanal);
            await _context.SaveChangesAsync();
            return Ok(yeniKanal);
        }

        // 🚀 YENİ: KANAL DÜZENLEME (Adını ve Türünü Değiştirme)
        [HttpPut("guncelle/{id}")]
        public async Task<IActionResult> KanalGuncelle(int id, [FromBody] KanalIslemiDto dto)
        {
            var kanal = await _context.Kanallar.FindAsync(id);
            if (kanal == null) return NotFound("Kanal bulunamadı.");

            kanal.KanalAdi = dto.KanalAdi.ToLower().Replace(" ", "-");
            kanal.Tip = dto.Tip;

            await _context.SaveChangesAsync();
            return Ok(kanal);
        }
        // KANAL SİLME
        [HttpDelete("sil/{id}")]
        public async Task<IActionResult> KanalSil(int id)
        {
            var kanal = await _context.Kanallar.FindAsync(id);
            if (kanal == null) return NotFound("Kanal bulunamadı.");

            _context.Kanallar.Remove(kanal);
            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Kanal başarıyla silindi." });
        }
        // SUNUCUNUN KANALLARINI GETİRME
        [HttpGet("sunucu/{sunucuId}")]
        public async Task<IActionResult> GetKanallar(int sunucuId)
        {
            var kanallar = await _context.Kanallar
                .Where(k => k.SunucuId == sunucuId)
                .ToListAsync();

            return Ok(kanallar);
        }
    }
}