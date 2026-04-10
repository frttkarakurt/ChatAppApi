using System.Text;
using ChatAppApi.Data;
using ChatAppApi.Hubs; // ChatHub sýnýfýnýn olduðu klasör
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

/////////// Db Connection
//sqllite
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
//postgresql
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// API Controller ve SignalR (Chat için) servislerini ekle
builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors();

// --- EKLENEN BÖLÜM: JWT KÝMLÝK DOÐRULAMA AYARLARI ---
// appsettings.json içinden gizli anahtarýmýzý okuyoruz
var secretKey = builder.Configuration.GetSection("AppSettings:Token").Value;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
            ValidateIssuer = false,
            ValidateAudience = false
        };

        // SignalR (WebSockets) token'ý HTTP baþlýðýnda (Header) deðil, URL'de taþýr.
        // Bu yüzden gelen token'ý URL'den yakalamamýz gerekir:
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chathub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });
// ----------------------------------------------------

var app = builder.Build();

// Routing ve Endpoint'leri ayarla
app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Güvenlik duvarlarýnýn sýralamasý son derece kritiktir!
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

// --- EKLENEN BÖLÜM: KÝMLÝK KONTROLÜ VE YETKÝLENDÝRME ---
app.UseAuthentication(); // "Sen kimsin?"
app.UseAuthorization();  // "Buraya girmeye yetkin var mý?"
// -------------------------------------------------------

app.MapControllers(); // Controller'larý aktif et

// --- EKLENEN BÖLÜM: SIGNALR SOHBET SANTRALÝ YÖNLENDÝRMESÝ ---
app.MapHub<ChatHub>("/chathub");
// ------------------------------------------------------------

app.MapGet("/", () => "Discord API Çalýþýyor!"); // Test için ana sayfa mesajý

app.Run();