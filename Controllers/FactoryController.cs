using ErtamIK.WebPortal.Models;
using ErtamIK.WebPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ErtamIK.WebPortal.Controllers;

[Authorize(Roles = "Factory")]
public sealed class FactoryController(PortalRepository repository) : Controller
{
    public async Task<IActionResult> Index()
    {
        int userId = GetUserId();
        ViewBag.Profile = await repository.GetFactoryProfileAsync(userId);
        return View(await repository.GetFactoryRequestsAsync(userId));
    }

    [HttpGet]
    public IActionResult NewRequest() => View(new StaffingRequestViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> NewRequest(StaffingRequestViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        await repository.AddStaffingRequestAsync(GetUserId(), model);
        TempData["Success"] = "Personel talebiniz Ertam İnsan Kaynakları'na iletildi ve güvenle kaydedildi.";
        return RedirectToAction(nameof(Index));
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
