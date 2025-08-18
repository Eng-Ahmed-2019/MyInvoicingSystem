using InvoicingSystem.Localization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Resources;

namespace InvoicingSystem.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required(
            ErrorMessageResourceType = typeof(Messages),
            ErrorMessageResourceName = "UsernameRequired")]
        [StringLength(50,
            ErrorMessageResourceType = typeof(Messages),
            ErrorMessageResourceName = "UsernameMaxLength")]
        [RegularExpression(@"^[a-zA-Z\s]+$",
            ErrorMessageResourceType = typeof(Messages),
            ErrorMessageResourceName = "EnglishLettersOnly")]
        public string Username { get; set; } = string.Empty;

        [Required(
            ErrorMessageResourceType = typeof(Messages),
            ErrorMessageResourceName = "EmailRequired")]
        [EmailAddress(
            ErrorMessageResourceType = typeof(Messages),
            ErrorMessageResourceName = "InvalidEmail")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(
            ErrorMessageResourceType = typeof(Messages),
            ErrorMessageResourceName = "PasswordRequired")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required(
            ErrorMessageResourceType = typeof(Messages),
            ErrorMessageResourceName = "FullNameRequired")]
        [StringLength(100,
            ErrorMessageResourceType = typeof(Messages),
            ErrorMessageResourceName = "FullNameMaxLength")]
        [RegularExpression(@"^[a-zA-Z\s]+$",
            ErrorMessageResourceType = typeof(Messages),
            ErrorMessageResourceName = "EnglishLettersOnly")]
        public string FullName { get; set; } = string.Empty;

        [Required(
            ErrorMessageResourceType = typeof(Messages),
            ErrorMessageResourceName = "FullNameArRequired")]
        [StringLength(100,
            ErrorMessageResourceType = typeof(Messages),
            ErrorMessageResourceName = "FullNameArMaxLength")]
        [ArabicLettersOnly(
            ErrorMessageResourceType = typeof(Messages),
            ErrorMessageResourceName = "ArabicLettersOnly")]
        public string FullNameAr { get; set; } = string.Empty;

        [Required(
            ErrorMessageResourceType = typeof(Messages),
            ErrorMessageResourceName = "CompanyRequired")]
        public Guid CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        [Required(
            ErrorMessageResourceType = typeof(Messages),
            ErrorMessageResourceName = "RoleRequired")]
        public Guid RoleId { get; set; }
        [ForeignKey(nameof(RoleId))]
        public Role? Role { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}