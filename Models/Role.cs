using InvoicingSystem.Localization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Resources;

namespace InvoicingSystem.Models
{
    public class Role
    {
        [Key]
        public Guid Id { get; set; }

        // New Update on my Task
        [Required(
            ErrorMessageResourceName = "Role_NameEn_Required",
            ErrorMessageResourceType = typeof(Messages))]
        [MaxLength(100,
            ErrorMessageResourceName = "Role_NameEn_MaxLength",
            ErrorMessageResourceType = typeof(Messages))]
        [RegularExpression(@"^[a-zA-Z\s]+$",
            ErrorMessageResourceName = "Role_NameEn_OnlyLetters",
            ErrorMessageResourceType = typeof(Messages))]
        public string NameEn { get; set; } = string.Empty;

        [Required(
            ErrorMessageResourceName = "Role_NameAr_Required",
            ErrorMessageResourceType = typeof(Messages))]
        [MaxLength(100,
            ErrorMessageResourceName = "Role_NameAr_MaxLength",
            ErrorMessageResourceType = typeof(Messages))]
        [ArabicLettersOnly(
            ErrorMessageResourceName = "Role_NameAr_OnlyLetters",
            ErrorMessageResourceType = typeof(Messages))]
        public string NameAr { get; set; } = string.Empty;

        [MaxLength(250,
            ErrorMessageResourceName = "Role_DescriptionEn_MaxLength",
            ErrorMessageResourceType = typeof(Messages))]
        [RegularExpression(@"^[a-zA-Z\s]+$",
            ErrorMessageResourceName = "Role_DescriptionEn_OnlyLetters",
            ErrorMessageResourceType = typeof(Messages))]
        public string? DescriptionEn { get; set; }

        [MaxLength(250,
            ErrorMessageResourceName = "Role_DescriptionAr_MaxLength",
            ErrorMessageResourceType = typeof(Messages))]
        [ArabicLettersOnly(
            ErrorMessageResourceName = "Role_DescriptionAr_OnlyLetters",
            ErrorMessageResourceType = typeof(Messages))]
        public string? DescriptionAr { get; set; }

        [Required]
        public Guid CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}