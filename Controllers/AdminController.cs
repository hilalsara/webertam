using ErtamIK.WebPortal.Models;
using ErtamIK.WebPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ErtamIK.WebPortal.Controllers;

[Authorize(Roles = "Admin")]
public sealed class AdminController(PortalRepository repository) : Controller
{
    public async Task<IActionResult> Index() => View(await repository.GetDashboardAsync());

    public async Task<IActionResult> Jobs() => View(await repository.GetAllJobsAsync());

    [HttpGet]
    public async Task<IActionResult> JobEdit(int id = 0)
    {
        if (id == 0) return View(new JobEditViewModel());
        Ilan? job = await repository.GetJobAsync(id);
        if (job is null) return NotFound();
        return View(new JobEditViewModel
        {
            Id = job.Id,
            Title = job.txtIlanBasligi,
            Sector = job.txtSektor,
            Location = job.txtSehirIlce,
            Detail = job.txtIlanDetay,
            IsActive = job.numIlanDurum == 1
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> JobEdit(JobEditViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        await repository.SaveJobAsync(model);
        TempData["Success"] = "İlan kaydedildi.";
        return RedirectToAction(nameof(Jobs));
    }

    public async Task<IActionResult> Applications() =>
        View(await repository.GetAllApplicationsAsync());

    public async Task<IActionResult> StaffingRequests() =>
        View(await repository.GetAllStaffingRequestsAsync());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> StaffingRequestStatus(int id, string status, string? adminNote)
    {
        await repository.UpdateStaffingRequestStatusAsync(id, status, adminNote);
        TempData["Success"] = "Müşteri personel talebi güncellendi.";
        return RedirectToAction(nameof(StaffingRequests));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ApplicationStatus(int id, string status)
    {
        await repository.UpdateApplicationStatusAsync(id, status);
        TempData["Success"] = "Başvuru durumu güncellendi.";
        return RedirectToAction(nameof(Applications));
    }

    public async Task<IActionResult> Blog() => View(await repository.GetAllPostsAsync());

    [HttpGet]
    public async Task<IActionResult> BlogEdit(int id = 0)
    {
        if (id == 0) return View(new BlogEditViewModel());
        BlogPost? post = await repository.GetPostAsync(id, true);
        if (post is null) return NotFound();
        return View(new BlogEditViewModel
        {
            Id = post.Id,
            Title = post.Title,
            Summary = post.Summary,
            Content = post.Content,
            Category = post.Category,
            IsPublished = true
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> BlogEdit(BlogEditViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        await repository.SavePostAsync(model);
        TempData["Success"] = "Blog/haber içeriği kaydedildi.";
        return RedirectToAction(nameof(Blog));
    }

    public async Task<IActionResult> Messages() => View(await repository.GetMessagesAsync());

    [HttpGet]
    public async Task<IActionResult> Content() => View(await repository.GetSiteContentAsync());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Content(SiteContent model)
    {
        await repository.SaveSiteContentAsync(model);
        TempData["Success"] = "Site metinleri ve iletişim bilgileri güncellendi.";
        return RedirectToAction(nameof(Content));
    }
}
