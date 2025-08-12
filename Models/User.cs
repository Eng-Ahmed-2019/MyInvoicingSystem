using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoicingSystem.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, ErrorMessage = "Username cannot be longer than 50 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "This field must contain English letters only.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "This field must contain English letters only.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full name in arabic is required")]
        [StringLength(100)]
        [ArabicLettersOnly]
        public string FullNameAr { get; set; } = string.Empty;

        [Required(ErrorMessage = "Company is required")]
        public Guid CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public Guid RoleId { get; set; }
        [ForeignKey(nameof(RoleId))]
        public Role? Role { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}