using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoicingSystem.Models
{
    public class Role
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "English name is required.")]
        [MaxLength(100, ErrorMessage = "English name cannot exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "This field must contain English letters only.")]
        public string NameEn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Arabic name is required.")]
        [MaxLength(100, ErrorMessage = "Arabic name cannot exceed 100 characters.")]
        [ArabicLettersOnly]
        public string NameAr { get; set; } = string.Empty;

        [MaxLength(250, ErrorMessage = "Description in English cannot exceed 250 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "This field must contain English letters only.")]
        public string? DescriptionEn { get; set; }

        [MaxLength(250, ErrorMessage = "Description in Arabic cannot exceed 250 characters.")]
        [ArabicLettersOnly]
        public string? DescriptionAr { get; set; }

        [Required]
        public Guid CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}