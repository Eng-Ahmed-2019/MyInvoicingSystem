using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoicingSystem.Models
{
    public class InvoiceItem
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid InvoiceId { get; set; }
        [ForeignKey(nameof(InvoiceId))]
        public Invoice? Invoice { get; set; }

        [Required]
        public Guid ItemId { get; set; }
        [ForeignKey(nameof(ItemId))]
        public Item? Item { get; set; }

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

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}