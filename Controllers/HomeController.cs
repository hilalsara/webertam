using ErtamIK.WebPortal.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Npgsql;
using System.Data.Common;
using System.Diagnostics;

namespace ErtamIK.WebPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            List<Ilan> ilanListesi = new();

            string connectionString = _configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection bağlantı bilgisi tanımlanmamış.");
            string databaseProvider = _configuration["DatabaseProvider"] ?? "MySql";

            try
            {
                using DbConnection baglanti = CreateConnection(databaseProvider, connectionString);
                using DbCommand komut = baglanti.CreateCommand();
                komut.CommandText = CreateListingQuery(databaseProvider);

                baglanti.Open();
                using DbDataReader oku = komut.ExecuteReader();
                while (oku.Read())
                {
                    ilanListesi.Add(new Ilan
                    {
                        Id = Convert.ToInt32(oku["id"]),
                        txtIlanBasligi = oku["txtIlanBasligi"].ToString(),
                        txtSektor = oku["txtSektor"].ToString(),
                        txtSehirIlce = oku["txtSehirIlce"].ToString(),
                        txtIlanDetay = oku["txtIlanDetay"].ToString(),
                        numIlanDurum = Convert.ToByte(oku["numIlanDurum"])
                    });
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "İlan veritabanına bağlanılamadı.");
            }

            return View(ilanListesi);
        }

        private static DbConnection CreateConnection(string provider, string connectionString)
        {
            return provider.Trim().ToLowerInvariant() switch
            {
                "postgresql" or "postgres" or "npgsql" => new NpgsqlConnection(connectionString),
                "mysql" or "mariadb" => new MySqlConnection(connectionString),
                _ => throw new InvalidOperationException(
                    $"Desteklenmeyen DatabaseProvider değeri: '{provider}'. MySql veya PostgreSql kullanın.")
            };
        }

        private static string CreateListingQuery(string provider)
        {
            if (provider.Trim().Equals("PostgreSql", StringComparison.OrdinalIgnoreCase) ||
                provider.Trim().Equals("Postgres", StringComparison.OrdinalIgnoreCase) ||
                provider.Trim().Equals("Npgsql", StringComparison.OrdinalIgnoreCase))
            {
                return """
                    SELECT id,
                           "txtIlanBasligi",
                           "txtSektor",
                           "txtSehirIlce",
                           "txtIlanDetay",
                           "numIlanDurum"
                    FROM ilanlar
                    WHERE "numIlanDurum" = 1
                    """;
            }

            return
                "SELECT id, txtIlanBasligi, txtSektor, txtSehirIlce, txtIlanDetay, numIlanDurum " +
                "FROM ilanlar WHERE numIlanDurum = 1";
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
