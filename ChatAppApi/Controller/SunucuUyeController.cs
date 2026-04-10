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
    public class SunucuUyeController : ControllerBase
    {
        private readonly AppDbContext _context;
        public SunucuUyeController(AppDbContext context) { _context = context; }
        [HttpGet("liste/{sunucuId}")]
        public async Task<IActionResult> GetSunucuUyeleri(int sunucuId)
        {
            var uyeler = await (from u in _context.SunucuUyeleri
                                join k in _context.Kullanicilar on u.KullaniciId equals k.Id
                                where u.SunucuId == sunucuId

                                // Rol tablosuyla renk eşleşmesi
                                join r in _context.Roller on new { u.SunucuId, RolAdi = u.Rol } equals new { r.SunucuId, r.RolAdi } into roller
                                from rolTablosu in roller.DefaultIfEmpty()

                                select new
                                {
                                    KullaniciId = u.KullaniciId,
                                    KullaniciAdi = k.Username,
                                    Rol = u.Rol,
                                    RenkHex = rolTablosu != null ? rolTablosu.RenkHex : "#8E9297"
                                }).ToListAsync();

            return Ok(uyeler);
        }

        [HttpPut("rol-ata")]
        public async Task<IActionResult> RolAta([FromBody] RolAtaDto dto)
        {
            var uye = await _context.SunucuUyeleri.FirstOrDefaultAsync(u => u.SunucuId == dto.SunucuId && u.KullaniciId == dto.KullaniciId);
            if (uye == null) return NotFound("Üye bulunamadı.");

            uye.Rol = dto.YeniRolAdi;
            await _context.SaveChangesAsync();
            return Ok(new { mesaj = "Rol başarıyla atandı!" });
        }

        public class RolAtaDto {
            public int SunucuId { get; set; }
            public int KullaniciId { get; set; }
            public string YeniRolAdi { get; set; }
        }
    }
}