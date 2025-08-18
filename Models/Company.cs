using InvoicingSystem.Localization;
using System.ComponentModel.DataAnnotations;

namespace InvoicingSystem.Models
{
    public class Company
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "CompanyNameRequired")]
        [MaxLength(150, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "CompanyNameMaxLength")]
        [MinLength(2, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "CompanyNameMinLength")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "CompanyNameEnglishOnly")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "CompanyNameRequired")]
        [MaxLength(150, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "CompanyNameMaxLength")]
        [MinLength(2, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "CompanyNameMinLength")]
        [ArabicLettersOnly(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "CompanyNameArabicOnly")]
        public string NameAr { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "CompanyDescriptionMaxLength")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "CompanyDescriptionEnglishOnly")]
        public string? Description { get; set; }

        [MaxLength(500, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "CompanyDescriptionMaxLength")]
        [ArabicLettersOnly(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "CompanyDescriptionArabicOnly")]
        public string? DescriptionAr { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; }

        public ICollection<Role> Roles { get; set; } = new List<Role>();
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Customer> Customers { get; set; } = new List<Customer>();
        public ICollection<Item> Items { get; set; } = new List<Item>();
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}