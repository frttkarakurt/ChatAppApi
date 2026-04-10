# Benchat API 🚀

Benchat, modern web teknolojileri ile geliştirilmiş, gerçek zamanlı iletişim odaklı bir backend servisidir. Bu proje, yerel pazardaki erişilebilirlik ihtiyaçlarını karşılamak üzere tasarlanmış kapsamlı bir mesajlaşma platformunun çekirdek (API) katmanını oluşturur.

## 🛠 Kullanılan Teknolojiler

* **Framework:** ASP.NET Core 8.0 (Web API)
* **Real-time:** SignalR (Anlık mesaj iletimi için)
* **Database & Auth:** Supabase (PostgreSQL tabanlı altyapı)
* **Architecture:** RESTful prensiplerine uygun mimari

## ✨ Özellikler

- [x] **Gerçek Zamanlı Mesajlaşma:** SignalR ile düşük gecikmeli veri iletimi.
- [x] **Kullanıcı Kimlik Doğrulama:** Supabase entegrasyonu ile güvenli giriş/kayıt işlemleri.
- [x] **Ölçeklenebilir API:** Clean Code prensipleriyle yazılmış, genişletilebilir endpoint yapıları.
- [ ] **Mesaj Şifreleme:** Uçtan uca şifreleme desteği (Geliştirme aşamasında).

## 🚀 Başlangıç

Projeyi yerel ortamınızda çalıştırmak için şu adımları izleyebilirsiniz:

1. **Repoyu klonlayın:**
   ```bash
   git clone [https://github.com/frttkarakurt/ChatAppApi.git](https://github.com/frttkarakurt/ChatAppApi.git)
Gerekli bağımlılıkları yükleyin:

Bash
dotnet restore

Yapılandırma:
appsettings.json dosyasını kendi Supabase URL ve Key bilgilerinizle güncelleyin.

⚖️ Lisans
Bu proje Apache License 2.0 ile lisanslanmıştır.
'@
