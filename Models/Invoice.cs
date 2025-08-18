using InvoicingSystem.Localization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Resources;

namespace InvoicingSystem.Models
{
    public class Invoice
    {
        [Key]
        public Guid Id { get; set; }

        [Required(
            ErrorMessageResourceType = typeof(Messages),
            ErrorMessageResourceName = "InvoiceNumber_Required"
        )]
        [StringLength(50)]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required(
            ErrorMessageResourceType = typeof(Messages),
            ErrorMessageResourceName = "Title_Required"
        )]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(
            ErrorMessageResourceType = typeof(Messages),
            ErrorMessageResourceName = "TitleAr_Required"
        )]
        [StringLength(200)]
        [ArabicLettersOnly]
        public string TitleAr { get; set; } = string.Empty;

        [StringLength(1000,
            ErrorMessageResourceType = typeof(Messages),
            ErrorMessageResourceName = "Description_TooLong"
        )]
        public string? Description { get; set; }

        [StringLength(1000,
            ErrorMessageResourceType = typeof(Messages),
            ErrorMessageResourceName = "Description_TooLong"
        )]
        [ArabicLettersOnly]
        public string? DescriptionAr { get; set; }

        [Required(
            ErrorMessageResourceType = typeof(Messages),
            ErrorMessageResourceName = "Customer_Required"
        )]
        public Guid CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public Customer? Customer { get; set; }

        [Required(
            ErrorMessageResourceType = typeof(Messages),
            ErrorMessageResourceName = "Company_Required"
        )]
        public Guid CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue,
            ErrorMessageResourceType = typeof(Messages),
            ErrorMessageResourceName = "Subtotal_Range"
        )]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue,
            ErrorMessageResourceType = typeof(Messages),
            ErrorMessageResourceName = "Tax_Range"
        )]
        public decimal? Tax { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue,
            ErrorMessageResourceType = typeof(Messages),
            ErrorMessageResourceName = "Total_Range"
        )]
        public decimal Total { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public Guid? CreatedByUserId { get; set; }
        [ForeignKey(nameof(CreatedByUserId))]
        public User? CreatedByUser { get; set; }

        public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    }
}