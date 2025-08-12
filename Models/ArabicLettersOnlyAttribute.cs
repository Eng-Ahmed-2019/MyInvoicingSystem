using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

namespace InvoicingSystem.Models
{
    public class ArabicLettersOnlyAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            var input = value.ToString();
            var regex = new Regex(@"^[\u0600-\u06FF\s]+$");

            if (!regex.IsMatch(input!))
            {
                return new ValidationResult("هذا الحقل يجب أن يحتوي على حروف عربية فقط.");
            }
            return ValidationResult.Success;
        }
    }
}