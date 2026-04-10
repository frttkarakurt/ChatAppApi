using ChatAppApi.Data;
using ChatAppApi.DTOs; // Senin oluşturduğun DTO klasörünü dahil ettik
using ChatAppApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // 1. KAYIT OLMA (REGISTER) - Artık senin RegisterDto'nu kullanıyor!
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (await _context.Kullanicilar.AnyAsync(k => k.Email == registerDto.Email || k.Username == registerDto.Username))
            {
                return BadRequest("Bu e-posta veya kullanıcı adı zaten kullanımda!");
            }

            // DTO'dan gelen verileri gerçek veritabanı modeline dönüştürüyoruz
            var yeniKullanici = new Kullanici
            {
                Email = registerDto.Email,
                Username = registerDto.Username,
                Password = registerDto.Password,
                KayitTarihi = DateTime.UtcNow
            };

            await _context.Kullanicilar.AddAsync(yeniKullanici);
            await _context.SaveChangesAsync();

            return Ok("Kayıt başarılı! Artık giriş yapabilirsiniz.");
        }

        // 2. GİRİŞ YAPMA (LOGIN) - Artık senin LoginDto'nu kullanıyor!
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.Email == loginDto.Email && k.Password == loginDto.Password);

            if (kullanici == null)
            {
                return Unauthorized("E-posta veya şifre hatalı!");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var keyString = _configuration["AppSettings:Token"];

            if (string.IsNullOrEmpty(keyString))
            {
                return StatusCode(500, "Sunucu hatası: JWT Anahtarı bulunamadı! (appsettings.json dosyasını kontrol edin)");
            }

            var key = Encoding.ASCII.GetBytes(keyString);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                            {
                    // 🚀 YENİ: Token'ın içine kullanıcının gerçek ID numarasını gizliyoruz
                    new Claim(ClaimTypes.NameIdentifier, kullanici.Id.ToString()),

                    new Claim(ClaimTypes.Name, kullanici.Username),
                    new Claim(ClaimTypes.Email, kullanici.Email)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { token = tokenString, username = kullanici.Username });
        }
    }
}