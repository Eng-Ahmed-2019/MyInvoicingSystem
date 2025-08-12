using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoicingSystem.Models
{
    public class Invoice
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Invoice number is required.")]
        [StringLength(50)]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "This field must contain English letters only.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Arabic title is required.")]
        [StringLength(200)]
        [ArabicLettersOnly]
        public string TitleAr { get; set; } = string.Empty;

        [StringLength(1000)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "This field must contain English letters only.")]
        public string? Description { get; set; }

        [StringLength(1000)]
        [ArabicLettersOnly]
        public string? DescriptionAr { get; set; }

        [Required]
        public Guid CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public Customer? Customer { get; set; }

        [Required]
        public Guid CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Subtotal must be greater than or equal to 0.")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Tax must be greater than or equal to 0.")]
        public decimal? Tax { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Total must be greater than or equal to 0.")]
        public decimal Total { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public Guid? CreatedByUserId { get; set; }
        [ForeignKey(nameof(CreatedByUserId))]
        public User? CreatedByUser { get; set; }

        public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    }
}