using Net.payOS;
using Net.payOS.Types;

namespace EV_SCMMS.Core.Application.Interfaces.Services;

public interface IPayOsClient
{
    Task<CreatePaymentResult> createPaymentLink(PaymentData paymentData);
    Task<PaymentLinkInformation> getPaymentLinkInformation(int orderCode);
    Task<PaymentLinkInformation> cancelPaymentLink(int orderCode);
    Task confirmWebhook(string webhookUrl);
    WebhookData verifyPaymentWebhookData(WebhookType webhook);
}
