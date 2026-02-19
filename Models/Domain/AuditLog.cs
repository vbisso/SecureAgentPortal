using System.ComponentModel.DataAnnotations;

namespace SecureAgentPortal.Models.Domain;

public class AuditLog
{
    public int Id { get; set; }

    [Required]
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

    [Required, StringLength(50)]
    public string Action { get; set; } = ""; // Created/Updated/Approved/Rejected/Deleted/Login

    [StringLength(50)]
    public string EntityType { get; set; } = "";

    public string? EntityId { get; set; }

    [Required]
    public string PerformedByUserId { get; set; } = "";

    [StringLength(45)]
    public string? IpAddress { get; set; }

    [StringLength(400)]
    public string? Details { get; set; }
}
