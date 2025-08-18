using InvoicingSystem.Data;
using InvoicingSystem.Models;

namespace InvoicingSystem.Services
{
    public static class DatabaseSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Companies.Any())
            {
                var companyId = Guid.Parse("11111111-1111-1111-1111-111111111111");
                var roleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
                var userId = Guid.Parse("33333333-3333-3333-3333-333333333333");

                var company = new Company
                {
                    Id = companyId,
                    Name = "Seeded Company",
                    NameAr = "شركة افتراضية",
                    Description = "Created automatically",
                    DescriptionAr = "تم الإنشاء تلقائيًا",
                    CreatedAt = DateTime.UtcNow
                };

                var role = new Role
                {
                    Id = roleId,
                    NameEn = "Admin",
                    NameAr = "مدير",
                    DescriptionEn = "Full access",
                    DescriptionAr = "صلاحيات كاملة",
                    CompanyId = companyId,
                    CreatedAt = DateTime.UtcNow
                };

                var user = new User
                {
                    Id = userId,
                    Username = "admin",
                    Email = "admin@example.com",
                    PasswordHash = PasswordHasher.HashPassword("123456789"),
                    FullName = "Admin User",
                    FullNameAr = "المسؤول",
                    CompanyId = companyId,
                    RoleId = roleId,
                    CreatedAt = DateTime.UtcNow
                };

                context.Companies.Add(company);
                context.Roles.Add(role);
                context.Users.Add(user);

                context.SaveChanges();
            }
        }
    }
}