using System.ComponentModel.DataAnnotations;

namespace SecureAgentPortal.Models.Domain;

public class Transaction
{
    public int Id { get; set; }

    [Required, StringLength(64)]
    public string ReferenceId { get; set; } = Guid.NewGuid().ToString("N");

    [Required, StringLength(80)]
    public string PayeeName { get; set; } = "";

    [Range(0.01, 999999.99)]
    public decimal Amount { get; set; }

    [Required, StringLength(3)]
    public string Currency { get; set; } = "USD";

    [Required]
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    [Required, StringLength(20)]
    public string Status { get; set; } = "Pending"; // Pending/Approved/Rejected

    [Required]
    public string CreatedByUserId { get; set; } = "";
}
