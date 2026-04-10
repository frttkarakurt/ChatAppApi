using ChatAppApi.Data;
using ChatAppApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace ChatAppApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RolController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RolController(AppDbContext context)
        {
            _context = context;
        }

        // Dışarıdan veri almak için basit bir model
        public class RolIslemiDto
        {
            public int SunucuId { get; set; }
            public string RolAdi { get; set; } = string.Empty;
            public string RenkHex { get; set; } = "#99AAB5"; // Varsayılan renk: Gri
        }

        // 1. YENİ ROL OLUŞTURMA
        [HttpPost("olustur")]
        public async Task<IActionResult> RolOlustur([FromBody] RolIslemiDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.RolAdi))
                return BadRequest("Rol adı boş olamaz.");

            var yeniRol = new Rol
            {
                SunucuId = dto.SunucuId,
                RolAdi = dto.RolAdi,
                RenkHex = dto.RenkHex
            };

            await _context.Roller.AddAsync(yeniRol);
            await _context.SaveChangesAsync();

            return Ok(yeniRol);
        }

        // 2. SUNUCUYA AİT ROLLERİ GETİRME
        [HttpGet("sunucu/{sunucuId}")]
        public async Task<IActionResult> GetRoller(int sunucuId)
        {
            var roller = await _context.Roller
                .Where(r => r.SunucuId == sunucuId)
                .ToListAsync();

            return Ok(roller);
        }

        // 3. ROL DÜZENLEME (YENİ EKLENDİ)
        [HttpPut("guncelle/{id}")]
        public async Task<IActionResult> RolGuncelle(int id, [FromBody] RolIslemiDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.RolAdi))
                return BadRequest("Rol adı boş olamaz.");

            var rol = await _context.Roller.FindAsync(id);
            if (rol == null) return NotFound("Rol bulunamadı.");

            // Yeni bilgileri üzerine yaz
            rol.RolAdi = dto.RolAdi;
            rol.RenkHex = dto.RenkHex;

            await _context.SaveChangesAsync();

            return Ok(rol);
        }
        // 4. ROL SİLME
        [HttpDelete("sil/{id}")]
        public async Task<IActionResult> RolSil(int id)
        {
            var rol = await _context.Roller.FindAsync(id);
            if (rol == null) return NotFound("Rol bulunamadı.");

            _context.Roller.Remove(rol);
            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Rol başarıyla silindi." });
        }
    }
}