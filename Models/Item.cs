using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoicingSystem.Models
{
    public class Item
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "This field must contain English letters only.")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        [ArabicLettersOnly]
        public string NameAr { get; set; } = string.Empty;

        [MaxLength(500)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "This field must contain English letters only.")]
        public string? Description { get; set; }

        [MaxLength(500)]
        [ArabicLettersOnly]
        public string? DescriptionAr { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Required]
        public Guid CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    }
}