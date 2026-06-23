using Microsoft.AspNetCore.Mvc;
using ErtamIK.WebPortal.Models;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace ErtamIK.WebPortal.Controllers
{
    public class HomeController : Controller
    {
        // Hem loglama hem de appsettings.json dosyasýný okumak için gerekli tanýmlamalar
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        // Ýkisini de tek bir yapýcý metotta (Constructor) birleþtiriyoruz
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        // Ana Sayfa (Ýlanlarýn Listeleneceði Yer)
        public IActionResult Index()
        {
            List<Ilan> ilanListesi = new List<Ilan>();

            // appsettings içindeki baðlantý dizesini alýyoruz
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            // MySQL baðlantýsýný baþlatýyoruz
            using (MySqlConnection baglanti = new MySqlConnection(connectionString))
            {
                // Sadece durumu 1 (Aktif) olan ilanlarý çekiyoruz
                string sorgu = "SELECT id, txtIlanBasligi, txtSektor, txtSehirIlce, txtIlanDetay, numIlanDurum FROM ilanlar WHERE numIlanDurum = 1";

                using (MySqlCommand komut = new MySqlCommand(sorgu, baglanti))
                {
                    baglanti.Open();
                    using (MySqlDataReader oku = komut.ExecuteReader())
                    {
                        while (oku.Read())
                        {
                            Ilan yeniIlan = new Ilan
                            {
                                Id = Convert.ToInt32(oku["id"]),
                                txtIlanBasligi = oku["txtIlanBasligi"].ToString(),
                                txtSektor = oku["txtSektor"].ToString(),
                                txtSehirIlce = oku["txtSehirIlce"].ToString(),
                                txtIlanDetay = oku["txtIlanDetay"].ToString(),
                                numIlanDurum = Convert.ToByte(oku["numIlanDurum"])
                            };
                            ilanListesi.Add(yeniIlan);
                        }
                    }
                }
            }

            // Çektiðimiz pýrýl pýrýl ilan listesini ön yüze (View) gönderiyoruz
            return View(ilanListesi);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}