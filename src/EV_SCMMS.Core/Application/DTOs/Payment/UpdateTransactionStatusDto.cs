using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.Payment;

public class UpdateTransactionStatusDto
{
    [Required]
    [StringLength(50)]
    public string Status { get; set; } = string.Empty;
}
