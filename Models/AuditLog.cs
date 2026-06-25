namespace EmployeeManagement.Models
{
    public class AuditLog
    {
        public int AuditLogId { get; set; }

        public string? Username { get; set; }

        public string? Action { get; set; }

        public string? Module { get; set; }

        public string? RecordInfo { get; set; }

        public DateTime ActionDate { get; set; }

        public string? PerformedBy { get; set; }
    }
}