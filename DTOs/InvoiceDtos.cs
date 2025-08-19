using InvoicingSystem.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoicingSystem.DTOs
{
    public class InvoiceItemDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string NameAr { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? DescriptionAr { get; set; }
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }
    }

    public class InvoiceCreateDto
    {
        public Guid CustomerId { get; set; }
        [Required]
        public string InvoiceNumber { get; set; } = string.Empty;
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string TitleAr { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? DescriptionAr { get; set; }
        public decimal? Tax { get; set; }
        [Required]
        public List<InvoiceItemDto> Items { get; set; } = new();
    }

    public class InvoiceReadDto
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? DescriptionAr { get; set; }
        public decimal Subtotal { get; set; }
        public decimal? Tax { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<InvoiceItemDto> Items { get; set; } = new();
    }
}