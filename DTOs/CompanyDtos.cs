using System.ComponentModel.DataAnnotations;

namespace InvoicingSystem.DTOs
{
    public class CompanyCreateDto
    {
        [Required(ErrorMessage = "اسم الشركة (بالإنجليزية) مطلوب.")]
        [MaxLength(150, ErrorMessage = "اسم الشركة (بالإنجليزية) يجب ألا يزيد عن 150 حرف.")]
        [MinLength(2, ErrorMessage = "اسم الشركة (بالإنجليزية) يجب ألا يقل عن حرفين.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "اسم الشركة (بالعربية) مطلوب.")]
        [MaxLength(150, ErrorMessage = "اسم الشركة (بالعربية) يجب ألا يزيد عن 150 حرف.")]
        [MinLength(2, ErrorMessage = "اسم الشركة (بالعربية) يجب ألا يقل عن حرفين.")]
        public string NameAr { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "الوصف (بالإنجليزية) يجب ألا يزيد عن 500 حرف.")]
        public string? Description { get; set; }

        [MaxLength(500, ErrorMessage = "الوصف (بالعربية) يجب ألا يزيد عن 500 حرف.")]
        public string? DescriptionAr { get; set; }
    }

    public class CompanyUpdateDto
    {
        [Required(ErrorMessage = "معرف الشركة مطلوب.")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "اسم الشركة (بالإنجليزية) مطلوب.")]
        [MaxLength(150, ErrorMessage = "اسم الشركة (بالإنجليزية) يجب ألا يزيد عن 150 حرف.")]
        [MinLength(2, ErrorMessage = "اسم الشركة (بالإنجليزية) يجب ألا يقل عن حرفين.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "اسم الشركة (بالعربية) مطلوب.")]
        [MaxLength(150, ErrorMessage = "اسم الشركة (بالعربية) يجب ألا يزيد عن 150 حرف.")]
        [MinLength(2, ErrorMessage = "اسم الشركة (بالعربية) يجب ألا يقل عن حرفين.")]
        public string NameAr { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "الوصف (بالإنجليزية) يجب ألا يزيد عن 500 حرف.")]
        public string? Description { get; set; }

        [MaxLength(500, ErrorMessage = "الوصف (بالعربية) يجب ألا يزيد عن 500 حرف.")]
        public string? DescriptionAr { get; set; }
    }

    public class CompanyReadDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? DescriptionAr { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}