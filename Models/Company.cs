using InvoicingSystem.Localization;
using System.ComponentModel.DataAnnotations;

namespace InvoicingSystem.Models
{
    public class Company
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "NameRequired")]
        [MaxLength(150, ErrorMessage = "اسم الشركة (بالإنجليزية) يجب ألا يزيد عن 150 حرف.")]
        [MinLength(2, ErrorMessage = "اسم الشركة (بالإنجليزية) يجب ألا يقل عن حرفين.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "This field must contain English letters only.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "اسم الشركة (بالعربية) مطلوب.")]
        [MaxLength(150, ErrorMessage = "اسم الشركة (بالعربية) يجب ألا يزيد عن 150 حرف.")]
        [MinLength(2, ErrorMessage = "اسم الشركة (بالعربية) يجب ألا يقل عن حرفين.")]
        [ArabicLettersOnly]
        public string NameAr { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "الوصف (بالإنجليزية) يجب ألا يزيد عن 500 حرف.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "This field must contain English letters only.")]
        public string? Description { get; set; }

        [MaxLength(500, ErrorMessage = "الوصف (بالعربية) يجب ألا يزيد عن 500 حرف.")]
        [ArabicLettersOnly]
        public string? DescriptionAr { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; }

        public ICollection<Role> Roles { get; set; } = new List<Role>();
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Customer> Customers { get; set; } = new List<Customer>();
        public ICollection<Item> Items { get; set; } = new List<Item>();
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}