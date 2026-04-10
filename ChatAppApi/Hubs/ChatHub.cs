using ChatAppApi.Data;
using ChatAppApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatAppApi.Hubs
{
    public class KullaniciOdasi
    {
        public string Username { get; set; } = string.Empty;
        public string Kanal { get; set; } = string.Empty;
    }

    [Authorize]
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;
        private static Dictionary<string, KullaniciOdasi> _aktifKullanicilar = new Dictionary<string, KullaniciOdasi>();

        public ChatHub(AppDbContext context)
        {
            _context = context;
        }

        // 🚀 1. UYGULAMA AÇILDIĞI AN TETİKLENİR
        public override async Task OnConnectedAsync()
        {
            _aktifKullanicilar[Context.ConnectionId] = new KullaniciOdasi
            {
                Username = Context.User?.Identity?.Name ?? "Bilinmeyen",
                Kanal = ""
            };

            // Birisi uygulamaya girdiğinde HERKESE güncel "online" listesini yolla
            await TumOnlineKullanicilariGonder();

            await base.OnConnectedAsync();
        }

        // 🚀 2. UYGULAMA KAPATILDIĞINDA TETİKLENİR
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (_aktifKullanicilar.ContainsKey(Context.ConnectionId))
            {
                _aktifKullanicilar.Remove(Context.ConnectionId);

                // Biri uygulamadan çıktığında HERKESE güncel listeyi yolla
                await TumOnlineKullanicilariGonder();
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task KanalaKatil(string kanalAdi)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, kanalAdi);

            if (_aktifKullanicilar.ContainsKey(Context.ConnectionId))
            {
                _aktifKullanicilar[Context.ConnectionId].Kanal = kanalAdi;
            }

            // Kanala giren kişiye o kanalın eski mesajlarını yolla
            var eskiMesajlar = await _context.Mesajlar
                .Where(m => m.KanalAdi == kanalAdi)
                .OrderBy(m => m.Tarih)
                .ToListAsync();

            await Clients.Caller.SendAsync("MesajGecmisiniYukle", eskiMesajlar);
        }

        public async Task KanaldanAyril(string kanalAdi)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, kanalAdi);
        }

        public async Task KanalaMesajGonder(string kanalAdi, string message)
        {
            var senderUsername = Context.User?.Identity?.Name ?? "Bilinmeyen";

            var yeniMesaj = new KayitliMesaj
            {
                SunucuAdi = "Genel",
                KanalAdi = kanalAdi,
                Gonderen = senderUsername,
                Icerik = message,
                Tarih = DateTime.UtcNow
            };

            await _context.Mesajlar.AddAsync(yeniMesaj);
            await _context.SaveChangesAsync();

            await Clients.Group(kanalAdi).SendAsync("ReceiveMessage", senderUsername, message);
        }

        public async Task YaziyorDurumu(string kanalAdi, bool yaziyorMu)
        {
            var username = Context.User?.Identity?.Name ?? "Bilinmeyen";
            await Clients.OthersInGroup(kanalAdi).SendAsync("BiriYaziyor", username, yaziyorMu);
        }

        // 🚀 3. YENİ YARDIMCI METOT: KANAL FARK ETMEKSİZİN TÜM ONLINE KİŞİLERİ YOLLAR
        private async Task TumOnlineKullanicilariGonder()
        {
            // Sadece benzersiz kullanıcı isimlerini (Username) alıyoruz
            var onlineOlanlar = _aktifKullanicilar.Values
                .Select(k => k.Username)
                .Distinct()
                .ToList();

            // Sadece bir kanala değil, uygulamaya bağlı HERKESE listeyi fırlat
            await Clients.All.SendAsync("AktifKullanicilariGuncelle", onlineOlanlar);
        }
        
        // Bir sunucuda ayar/kanal değiştiğinde tüm üyelere sinyal gönderir
        public async Task SunucuGuncellendiSinyaliGonder(int sunucuId)
        {
            // Bu sinyali alan tüm istemciler (Global veya grup bazlı yollanabilir)
            // Şimdilik basitlik adına herkese yolluyoruz, MainWindow'da filtreleyeceğiz.
            await Clients.All.SendAsync("SunucuYenile", sunucuId);
        }
    }
}