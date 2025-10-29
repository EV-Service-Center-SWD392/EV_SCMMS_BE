namespace EV_SCMMS.Core.Application.DTOs.Payment;


public record Response(
    int error,
    String message,
    object? data
);