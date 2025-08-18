using InvoicingSystem.Localization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Resources;

namespace InvoicingSystem.Models
{
    public class Item
    {
        [Key]
        public Guid Id { get; set; }

        [Required(
            ErrorMessageResourceName = "NameRequired",
            ErrorMessageResourceType = typeof(Messages))]
        [MaxLength(200,
            ErrorMessageResourceName = "NameMaxLength",
            ErrorMessageResourceType = typeof(Messages))]
        [RegularExpression(@"^[a-zA-Z\s]+$",
            ErrorMessageResourceName = "NameEnglishOnly",
            ErrorMessageResourceType = typeof(Messages))]
        public string Name { get; set; } = string.Empty;

        [Required(
            ErrorMessageResourceName = "NameArRequired",
            ErrorMessageResourceType = typeof(Messages))]
        [MaxLength(200,
            ErrorMessageResourceName = "NameArMaxLength",
            ErrorMessageResourceType = typeof(Messages))]
        [ArabicLettersOnly(
            ErrorMessageResourceName = "NameArArabicOnly",
            ErrorMessageResourceType = typeof(Messages))]
        public string NameAr { get; set; } = string.Empty;

        [MaxLength(500,
            ErrorMessageResourceName = "NameMaxLength",
            ErrorMessageResourceType = typeof(Messages))]
        [RegularExpression(@"^[a-zA-Z\s]+$",
            ErrorMessageResourceName = "DescEnglishOnly",
            ErrorMessageResourceType = typeof(Messages))]
        public string? Description { get; set; }

        [MaxLength(500,
            ErrorMessageResourceName = "NameArMaxLength",
            ErrorMessageResourceType = typeof(Messages))]
        [ArabicLettersOnly(
            ErrorMessageResourceName = "DescArArabicOnly",
            ErrorMessageResourceType = typeof(Messages))]
        public string? DescriptionAr { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue,
            ErrorMessageResourceName = "PriceRange",
            ErrorMessageResourceType = typeof(Messages))]
        public decimal UnitPrice { get; set; }

        [Required(
            ErrorMessageResourceName = "CompanyRequired",
            ErrorMessageResourceType = typeof(Messages))]
        public Guid CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    }
}