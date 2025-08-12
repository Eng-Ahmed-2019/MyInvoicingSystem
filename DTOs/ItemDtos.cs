namespace InvoicingSystem.DTOs
{
    public class ItemCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? DescriptionAr { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class ItemUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? DescriptionAr { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class ItemReadDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? DescriptionAr { get; set; }
        public decimal UnitPrice { get; set; }
    }
}