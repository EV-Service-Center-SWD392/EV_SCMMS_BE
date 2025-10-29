namespace EV_SCMMS.Core.Application.Interfaces;

/// <summary>
/// Interface for service result pattern
/// </summary>
public interface IServiceResult
{
    bool IsSuccess { get; }
    string? Message { get; }
    IEnumerable<string>? Errors { get; }
}

/// <summary>
/// Interface for service result pattern with data
/// </summary>
/// <typeparam name="T">Type of data</typeparam>
public interface IServiceResult<T> : IServiceResult
{
    T? Data { get; }
}
