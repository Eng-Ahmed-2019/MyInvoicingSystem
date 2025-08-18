using InvoicingSystem.Localization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoicingSystem.Models
{
    public class Customer
    {
        [Key]
        public Guid Id { get; set; }

        // New Update on my Task
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "CustomerNameRequired")]
        [MaxLength(200, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "CustomerNameMaxLength")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "CustomerNameEnglishOnly")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "CustomerNameArabicRequired")]
        [MaxLength(200, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "CustomerNameMaxLength")]
        [ArabicLettersOnly]
        public string NameAr { get; set; } = string.Empty;

        [EmailAddress(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "CustomerEmailInvalid")]
        public string? Email { get; set; }

        [Phone(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "CustomerPhoneInvalid")]
        [MaxLength(20)]
        public string? Phone { get; set; }

        [Required]
        public Guid CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}