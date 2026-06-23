# Ertam İK Web Portal

ASP.NET Core 8 MVC uygulaması. Yerelde MySQL/MariaDB, canlı ortamda PostgreSQL
ile çalışabilir.

## Ücretsiz canlı yayın düzeni

- Web uygulaması: Render Free Web Service
- Veritabanı: Neon Free PostgreSQL
- Domain ve DNS: Natro
- HTTPS: Render tarafından otomatik

## 1. Neon veritabanı

1. https://console.neon.tech adresinde ücretsiz bir proje oluşturun.
2. Neon SQL Editor ekranında `database-postgresql.sql` dosyasını çalıştırın.
3. Neon'un verdiği bağlantı metnini kopyalayın. `sslmode=require` içermelidir.
4. Mevcut ilanlar varsa tabloya aktarın.

Örnek ilan:

```sql
INSERT INTO ilanlar
("txtIlanBasligi", "txtSektor", "txtSehirIlce", "txtIlanDetay", "numIlanDurum")
VALUES
('Yazılım Uzmanı', 'Yazılım', 'Gebze / Kocaeli', 'İlan açıklaması', 1);
```

## 2. Render yayını

1. Bu klasörü bir GitHub deposuna gönderin.
2. https://dashboard.render.com adresinde **New > Blueprint** seçin.
3. GitHub deposunu bağlayın. Render `render.yaml` dosyasını okuyacaktır.
4. `ConnectionStrings__DefaultConnection` sorulduğunda Neon bağlantı metnini
   girin. Bu değer gizli ortam değişkeni olarak saklanır.
5. Deploy tamamlanınca verilen `onrender.com` adresini kontrol edin.

## 3. Natro domain bağlantısı

1. Render servisinde **Settings > Custom Domains > Add Custom Domain** ile
   domaini ekleyin.
2. Render'ın ekranda gösterdiği DNS hedeflerini not edin.
3. Natro müşteri panelinde **DNS Yönetimi** bölümüne girin.
4. `www` için Render'ın verdiği hedefe bir `CNAME` kaydı ekleyin.
5. Kök domain (`@`) için Render'ın gösterdiği `A` kaydını ekleyin.
6. Çakışan eski `A`, `CNAME` ve özellikle `AAAA` kayıtlarını kaldırın.
7. Render ekranına dönüp **Verify** düğmesine basın.

DNS değişikliklerinin görünmesi birkaç dakika ile 24 saat sürebilir. HTTPS
sertifikasını Render otomatik oluşturur.

## Ortam ayarları

Canlı ortam:

```text
ASPNETCORE_ENVIRONMENT=Production
DatabaseProvider=PostgreSql
ConnectionStrings__DefaultConnection=<Neon bağlantı metni>
```

Yerelde MySQL kullanılıyorsa `appsettings.Development.json` içine gerçek
MySQL bağlantı metni eklenmeli ve `DatabaseProvider` değeri `MySql` olmalıdır.
