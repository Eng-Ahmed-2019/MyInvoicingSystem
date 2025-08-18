using InvoicingSystem.Localization;
using Microsoft.AspNetCore.Components.Forms;
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

        [Required(
            ErrorMessageResourceName = "InvoiceItem_Name_Required",
            ErrorMessageResourceType = typeof(Messages))]
        [MaxLength(200)]
        [RegularExpression(@"^[a-zA-Z\s]+$",
            ErrorMessageResourceName = "InvoiceItem_Name_Invalid",
            ErrorMessageResourceType = typeof(Messages))]
        public string Name { get; set; } = string.Empty;

        [Required(
            ErrorMessageResourceName = "InvoiceItem_NameAr_Required",
            ErrorMessageResourceType = typeof(Messages))]
        [MaxLength(200)]
        [ArabicLettersOnly]
        public string NameAr { get; set; } = string.Empty;

        [MaxLength(500)]
        [RegularExpression(@"^[a-zA-Z\s]+$",
            ErrorMessageResourceName = "InvoiceItem_Description_Invalid",
            ErrorMessageResourceType = typeof(Messages))]
        public string? Description { get; set; }

        [MaxLength(500)]
        [ArabicLettersOnly(
            ErrorMessageResourceName = "InvoiceItem_DescriptionAr_Invalid",
            ErrorMessageResourceType = typeof(Messages))]
        public string? DescriptionAr { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessageResourceName = "InvoiceItem_Quantity_Range",
            ErrorMessageResourceType = typeof(Messages))]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue,
            ErrorMessageResourceName = "InvoiceItem_UnitPrice_Range",
            ErrorMessageResourceType = typeof(Messages))]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}