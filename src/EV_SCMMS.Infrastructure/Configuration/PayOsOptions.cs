namespace EV_SCMMS.Infrastructure.Configuration;

public class PayOsOptions
{
    public string? PAYOS_CLIENT_ID { get; set; }
    public string? PAYOS_API_KEY { get; set; }
    public string? PAYOS_CHECKSUM_KEY { get; set; }

    // Optional URLs for redirects
    public string? ReturnUrl { get; set; }
    public string? CancelUrl { get; set; }
}
