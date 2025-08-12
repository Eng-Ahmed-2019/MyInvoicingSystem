﻿namespace InvoicingSystem.DTOs
{
    public class CompantDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string DescriptionAr { get; set; } = string.Empty;
    }

    public class CompanyResultDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DescriptionAr { get; set; } = string.Empty;
    }
}