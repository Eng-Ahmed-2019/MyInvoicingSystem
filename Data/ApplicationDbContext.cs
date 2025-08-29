using InvoicingSystem.Models;
using InvoicingSystem.Services;
using Microsoft.EntityFrameworkCore;

namespace InvoicingSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Company> Companies { set; get; }
        public DbSet<Role> Roles { set; get; }
        public DbSet<User> Users { set; get; }
        public DbSet<Customer> Customers { set; get; }
        public DbSet<Item> Items { set; get; }
        public DbSet<Invoice> Invoices { set; get; }
        public DbSet<InvoiceItem> InvoiceItems { set; get; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        public override int SaveChanges()
        {
            AddAuditLogs();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddAuditLogs();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void AddAuditLogs()
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditLog>();

            foreach (var entry in ChangeTracker.Entries()
                .Where(e=>e.State==EntityState.Added 
                || e.State==EntityState.Deleted
                || e.State == EntityState.Modified))
            {
                var audit = new AuditLog
                {
                    TableName = entry.Entity.GetType().Name,
                    Action = entry.State.ToString(),
                    KeyValues = entry.Properties
                        .FirstOrDefault(p => p.Metadata.IsPrimaryKey())?.CurrentValue?.ToString(),
                    CreatedAt = DateTime.UtcNow
                };

                if (entry.State == EntityState.Modified)
                {
                    var oldValues = new Dictionary<string, object?>();
                    var newValues = new Dictionary<string, object?>();

                    foreach(var prop in entry.Properties)
                    {
                        if (prop.IsModified)
                        {
                            oldValues[prop.Metadata.Name] = prop.OriginalValue;
                            newValues[prop.Metadata.Name] = prop.CurrentValue;
                        }
                    }

                    audit.OldValues = System.Text.Json.JsonSerializer.Serialize(oldValues);
                    audit.NewValues = System.Text.Json.JsonSerializer.Serialize(newValues);
                }
                else if(entry.State == EntityState.Added)
                {
                    var newValues = entry.Properties.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);
                    audit.NewValues = System.Text.Json.JsonSerializer.Serialize(newValues);
                }
                else if (entry.State == EntityState.Deleted)
                {
                    var oldValues = entry.Properties.ToDictionary(p => p.Metadata.Name, p => p.OriginalValue);
                    audit.OldValues = System.Text.Json.JsonSerializer.Serialize(oldValues);
                }
                auditEntries.Add(audit);
            }

            if (auditEntries.Any())
            {
                AuditLogs.AddRange(auditEntries);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Company)
                .WithMany()
                .HasForeignKey(u => u.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Role>()
                .HasOne(r => r.Company)
                .WithMany()
                .HasForeignKey(r => r.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Customer)
                .WithMany()
                .HasForeignKey(i => i.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Company)
                .WithMany()
                .HasForeignKey(i => i.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        // Seed Data
        public void SeedDatabase()
        {
            DatabaseSeeder.Seed(this);
        }
    }
}