using ErtamIK.WebPortal.Models;
using ErtamIK.WebPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ErtamIK.WebPortal.Controllers;

[Authorize]
public sealed class ApplicationsController(PortalRepository repository) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Apply(int id)
    {
        Ilan? job = await repository.GetJobAsync(id);
        if (job is null || job.numIlanDurum != 1) return NotFound();

        return View(new ApplicationViewModel
        {
            JobId = job.Id,
            JobTitle = job.txtIlanBasligi,
            FullName = User.Identity?.Name ?? "",
            Email = User.FindFirstValue(ClaimTypes.Email) ?? ""
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Apply(ApplicationViewModel model)
    {
        Ilan? job = await repository.GetJobAsync(model.JobId);
        if (job is null || job.numIlanDurum != 1) return NotFound();
        model.JobTitle = job.txtIlanBasligi;
        if (!ModelState.IsValid) return View(model);

        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        string reference = await repository.AddApplicationAsync(userId, model);
        TempData["ReferenceCode"] = reference;
        return RedirectToAction(nameof(Success));
    }

    public IActionResult Success() =>
        TempData["ReferenceCode"] is null ? RedirectToAction("Index", "Home") : View();

    public async Task<IActionResult> MyApplications()
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return View(await repository.GetUserApplicationsAsync(userId));
    }
}
