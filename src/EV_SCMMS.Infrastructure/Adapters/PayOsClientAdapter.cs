using EV_SCMMS.Core.Application.Interfaces.Services;
using Net.payOS;
using Net.payOS.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Reflection;
using EV_SCMMS.Infrastructure.Configuration;

namespace EV_SCMMS.Infrastructure.Adapters;

public class PayOsClientAdapter : IPayOsClient
{
    private readonly PayOS _payOS;
    private readonly PayOsOptions _options;

    public PayOsClientAdapter(IConfiguration configuration, IOptions<PayOsOptions> options)
    {
        _options = options.Value;

        // Prefer using options but fallback to IConfiguration for legacy keys
        var clientId = _options.PAYOS_CLIENT_ID ?? configuration["PayOs:PAYOS_CLIENT_ID"];
        var apiKey = _options.PAYOS_API_KEY ?? configuration["PayOs:PAYOS_API_KEY"];
        var checksum = _options.PAYOS_CHECKSUM_KEY ?? configuration["PayOs:PAYOS_CHECKSUM_KEY"];

        // Try direct construction using a typical SDK constructor: new PayOS(clientId, apiKey, checksum)
        _payOS = new PayOS(clientId!, apiKey!, checksum!);
  }

    private void TrySetPropertyOrField(object target, string name, string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return;

        var type = target.GetType();
        var prop = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (prop != null && prop.CanWrite && prop.PropertyType == typeof(string))
        {
            prop.SetValue(target, value);
            return;
        }

        var field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (field != null && field.FieldType == typeof(string))
        {
            field.SetValue(target, value);
            return;
        }
    }

    public async Task<CreatePaymentResult> createPaymentLink(PaymentData paymentData)
    {
        return await _payOS.createPaymentLink(paymentData);
    }

    public async Task<PaymentLinkInformation> getPaymentLinkInformation(int orderCode)
    {
        return await _payOS.getPaymentLinkInformation(orderCode);
    }

    public async Task<PaymentLinkInformation> cancelPaymentLink(int orderCode)
    {
        return await _payOS.cancelPaymentLink(orderCode);
    }

    public async Task confirmWebhook(string webhookUrl)
    {
        await _payOS.confirmWebhook(webhookUrl);
    }

    public WebhookData verifyPaymentWebhookData(WebhookType webhook)
    {
        return _payOS.verifyPaymentWebhookData(webhook);
    }
}
