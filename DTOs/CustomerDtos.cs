namespace InvoicingSystem.DTOs
{
    public class CustomerCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }

    public class CustomerUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }

    public class CustomerReadDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}