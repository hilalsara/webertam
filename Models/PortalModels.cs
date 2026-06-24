using System.ComponentModel.DataAnnotations;

namespace ErtamIK.WebPortal.Models;

public sealed class UserAccount
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = "User";
}

public sealed class BlogPost
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Summary { get; set; } = "";
    public string Content { get; set; } = "";
    public string Category { get; set; } = "Haber";
    public DateTime PublishedAt { get; set; }
}

public sealed class JobApplication
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public int UserId { get; set; }
    public string JobTitle { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string CoverLetter { get; set; } = "";
    public string ReferenceCode { get; set; } = "";
    public string Status { get; set; } = "Alındı";
    public DateTime CreatedAt { get; set; }
}

public sealed class ContactMessage
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Subject { get; set; } = "";
    public string Message { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

public sealed class SiteContent
{
    public string PrivacyText { get; set; } = "";
    public string ContactAddress { get; set; } = "";
    public string ContactPhone { get; set; } = "";
    public string ContactEmail { get; set; } = "";
}

public sealed class HomeViewModel
{
    public IReadOnlyList<Ilan> Jobs { get; set; } = [];
    public IReadOnlyList<BlogPost> Posts { get; set; } = [];
}

public sealed class RegisterViewModel
{
    [Required, StringLength(120)]
    [Display(Name = "Ad Soyad")]
    public string FullName { get; set; } = "";

    [Required, EmailAddress]
    [Display(Name = "E-posta")]
    public string Email { get; set; } = "";

    [Required, MinLength(8)]
    [DataType(DataType.Password)]
    [Display(Name = "Şifre")]
    public string Password { get; set; } = "";

    [Required, Compare(nameof(Password))]
    [DataType(DataType.Password)]
    [Display(Name = "Şifre Tekrar")]
    public string ConfirmPassword { get; set; } = "";
}

public sealed class LoginViewModel
{
    [Required, EmailAddress]
    [Display(Name = "E-posta")]
    public string Email { get; set; } = "";

    [Required, DataType(DataType.Password)]
    [Display(Name = "Şifre")]
    public string Password { get; set; } = "";

    [Display(Name = "Beni hatırla")]
    public bool RememberMe { get; set; }
}

public sealed class ApplicationViewModel
{
    public int JobId { get; set; }
    public string JobTitle { get; set; } = "";

    [Required, StringLength(120)]
    [Display(Name = "Ad Soyad")]
    public string FullName { get; set; } = "";

    [Required, EmailAddress]
    [Display(Name = "E-posta")]
    public string Email { get; set; } = "";

    [Required, Phone, StringLength(30)]
    [Display(Name = "Telefon")]
    public string Phone { get; set; } = "";

    [Required, StringLength(3000, MinimumLength = 20)]
    [Display(Name = "Ön Yazı")]
    public string CoverLetter { get; set; } = "";
}

public sealed class ApplicationLookupViewModel
{
    [Required]
    [Display(Name = "Başvuru Kodu")]
    public string ReferenceCode { get; set; } = "";

    [Required, EmailAddress]
    [Display(Name = "E-posta")]
    public string Email { get; set; } = "";

    public JobApplication? Result { get; set; }
}

public sealed class ContactViewModel
{
    [Required, StringLength(120)]
    [Display(Name = "Ad Soyad")]
    public string FullName { get; set; } = "";

    [Required, EmailAddress]
    [Display(Name = "E-posta")]
    public string Email { get; set; } = "";

    [Phone, StringLength(30)]
    [Display(Name = "Telefon")]
    public string Phone { get; set; } = "";

    [Required, StringLength(160)]
    [Display(Name = "Konu")]
    public string Subject { get; set; } = "";

    [Required, StringLength(3000, MinimumLength = 10)]
    [Display(Name = "Mesaj")]
    public string Message { get; set; } = "";

    public SiteContent Contact { get; set; } = new();
}

public sealed class AdminDashboardViewModel
{
    public int UserCount { get; set; }
    public int ApplicationCount { get; set; }
    public int ActiveJobCount { get; set; }
    public int MessageCount { get; set; }
    public IReadOnlyList<JobApplication> RecentApplications { get; set; } = [];
}

public sealed class JobEditViewModel
{
    public int Id { get; set; }

    [Required, StringLength(250)]
    [Display(Name = "İlan Başlığı")]
    public string Title { get; set; } = "";

    [Required, StringLength(150)]
    [Display(Name = "Sektör")]
    public string Sector { get; set; } = "";

    [Required, StringLength(150)]
    [Display(Name = "Şehir / İlçe")]
    public string Location { get; set; } = "";

    [Required, StringLength(5000)]
    [Display(Name = "İlan Detayı")]
    public string Detail { get; set; } = "";

    [Display(Name = "Aktif")]
    public bool IsActive { get; set; } = true;
}

public sealed class BlogEditViewModel
{
    public int Id { get; set; }

    [Required, StringLength(220)]
    [Display(Name = "Başlık")]
    public string Title { get; set; } = "";

    [Required, StringLength(500)]
    [Display(Name = "Özet")]
    public string Summary { get; set; } = "";

    [Required, StringLength(10000)]
    [Display(Name = "İçerik")]
    public string Content { get; set; } = "";

    [Required, StringLength(80)]
    [Display(Name = "Kategori")]
    public string Category { get; set; } = "Haber";

    [Display(Name = "Yayında")]
    public bool IsPublished { get; set; } = true;
}
