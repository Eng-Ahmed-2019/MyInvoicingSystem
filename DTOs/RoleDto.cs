namespace InvoicingSystem.DTOs
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DescriptionAr { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class RoleReadDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string DescriptionAr { get; set; } = string.Empty;
    }
}