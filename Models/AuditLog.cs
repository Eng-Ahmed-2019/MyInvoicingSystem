using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoicingSystem.Models
{
    public class AuditLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string TableName { get; set; } = string.Empty;

        public string Action { get; set; } = string.Empty;

        public string? KeyValues { get; set; }

        public string? OldValues { get; set; }

        public string? NewValues { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}