using System.ComponentModel.DataAnnotations;

namespace EV_SCMMS.Core.Application.DTOs.Payment;

public class CreateTransactionDto
{
    [Required]
    public Guid OrderId { get; set; }

    public Guid? StaffId { get; set; }

    public int PaymentMethodId { get; set; }

}
