namespace InvoicingSystem.DTOs
{
    public class InvoiceItemCreateDto
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? DescriptionAr { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}