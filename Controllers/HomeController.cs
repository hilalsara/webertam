using ErtamIK.WebPortal.Models;
using ErtamIK.WebPortal.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ErtamIK.WebPortal.Controllers;

public sealed class HomeController(
    PortalRepository repository,
    ILogger<HomeController> logger) : Controller
{
    public async Task<IActionResult> Index(string? search, string? location)
    {
        try
        {
            IReadOnlyList<Ilan> jobs = await repository.GetActiveJobsAsync();
            if (!string.IsNullOrWhiteSpace(search))
                jobs = jobs.Where(x => x.txtIlanBasligi.Contains(search, StringComparison.OrdinalIgnoreCase)
                    || x.txtSektor.Contains(search, StringComparison.OrdinalIgnoreCase)
                    || x.txtIlanDetay.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(location))
                jobs = jobs.Where(x => x.txtSehirIlce.Contains(location, StringComparison.OrdinalIgnoreCase)).ToList();

            return View(new HomeViewModel
            {
                Jobs = jobs,
                Posts = await repository.GetPublishedPostsAsync(3),
                Search = search?.Trim() ?? "",
                Location = location?.Trim() ?? ""
            });
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Ana sayfa verileri yüklenemedi.");
            return View(new HomeViewModel());
        }
    }

    public async Task<IActionResult> Privacy() =>
        View(await repository.GetSiteContentAsync());

    public async Task<IActionResult> Blog() =>
        View(await repository.GetPublishedPostsAsync());

    public async Task<IActionResult> BlogDetail(int id)
    {
        BlogPost? post = await repository.GetPostAsync(id);
        return post is null ? NotFound() : View(post);
    }

    [HttpGet]
    public async Task<IActionResult> Contact() =>
        View(new ContactViewModel { Contact = await repository.GetSiteContentAsync() });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Contact(ContactViewModel model)
    {
        model.Contact = await repository.GetSiteContentAsync();
        if (!ModelState.IsValid) return View(model);

        await repository.AddContactMessageAsync(model);
        TempData["Success"] = "Mesajınız bize ulaştı. En kısa sürede sizinle iletişime geçeceğiz.";
        return RedirectToAction(nameof(Contact));
    }

    [HttpGet]
    public IActionResult ApplicationStatus() => View(new ApplicationLookupViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ApplicationStatus(ApplicationLookupViewModel model)
    {
        if (ModelState.IsValid)
        {
            model.Result = await repository.FindApplicationAsync(model.ReferenceCode, model.Email);
            if (model.Result is null)
                ModelState.AddModelError("", "Bu bilgilerle eşleşen bir başvuru bulunamadı.");
        }
        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
    });
}
