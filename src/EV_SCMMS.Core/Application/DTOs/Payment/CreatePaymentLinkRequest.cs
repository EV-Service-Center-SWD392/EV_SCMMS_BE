namespace EV_SCMMS.Core.Application.DTOs.Payment;


public record CreatePaymentLinkRequest(
    string productName,
    string description,
    int price,
    string returnUrl,
    string cancelUrl
);