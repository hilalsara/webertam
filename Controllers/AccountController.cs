using ErtamIK.WebPortal.Models;
using ErtamIK.WebPortal.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Security.Claims;

namespace ErtamIK.WebPortal.Controllers;

public sealed class AccountController(
    PortalRepository repository,
    PasswordService passwordService) : Controller
{
    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        if (await repository.FindUserByEmailAsync(model.Email) is not null)
        {
            ModelState.AddModelError(nameof(model.Email), "Bu e-posta adresi zaten kayıtlı.");
            return View(model);
        }

        try
        {
            int id = await repository.CreateUserAsync(model, passwordService.Hash(model.Password));
            await SignInAsync(new UserAccount
            {
                Id = id,
                FullName = model.FullName,
                Email = model.Email,
                Role = "User"
            }, false);
            return RedirectToAction("Index", "Home");
        }
        catch (PostgresException exception) when (exception.SqlState == "23505")
        {
            ModelState.AddModelError(nameof(model.Email), "Bu e-posta adresi zaten kayıtlı.");
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid) return View(model);

        UserAccount? user = await repository.FindUserByEmailAsync(model.Email);
        if (user is null || !passwordService.Verify(model.Password, user.PasswordHash))
        {
            ModelState.AddModelError("", "E-posta veya şifre hatalı.");
            return View(model);
        }

        await SignInAsync(user, model.RememberMe);
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return user.Role == "Admin"
            ? RedirectToAction("Index", "Admin")
            : RedirectToAction("Index", "Home");
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied() => View();

    private async Task SignInAsync(UserAccount user, bool persistent)
    {
        Claim[] claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        ];
        ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties
            {
                IsPersistent = persistent,
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(persistent ? 72 : 8)
            });
    }
}
