namespace InvoicingSystem.Localization
{
    public class Messages
    {
        public static string CompanyNameRequired => "Company name is required";
        public static string CompanyNameMaxLength => "Company name must be less than 150 characters";
        public static string CompanyNameMinLength => "Company name must be at least 2 characters";
        public static string CompanyNameEnglishOnly => "Company name must contain only English letters";
        public static string CompanyNameArabicOnly => "Company name must contain only Arabic letters";
        public static string CompanyDescriptionMaxLength => "Description must be less than 500 characters";
        public static string CompanyDescriptionEnglishOnly => "Description must contain only English letters";
        public static string CompanyDescriptionArabicOnly => "Description must contain only Arabic letters";
    }
}