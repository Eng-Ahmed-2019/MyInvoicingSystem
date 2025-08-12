using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoicingSystem.Models
{
    public class Customer
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Customer name is required")]
        [MaxLength(200)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "This field must contain English letters only.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Customer Arabic name is required")]
        [MaxLength(200)]
        [ArabicLettersOnly]
        public string NameAr { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
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