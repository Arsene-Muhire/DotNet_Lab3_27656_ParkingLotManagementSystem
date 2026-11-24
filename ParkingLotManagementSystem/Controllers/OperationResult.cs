namespace ParkingLotManagementSystem.Controllers;

public class OperationResult
{
    protected OperationResult(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    public bool Success { get; }
    public string Message { get; }

    public static OperationResult Ok(string message = "") => new(true, message);

    public static OperationResult Fail(string message) => new(false, message);
}

public class OperationResult<T> : OperationResult
{
    private OperationResult(bool success, string message, T? data) : base(success, message)
    {
        Data = data;
    }

    public T? Data { get; }

    public static OperationResult<T> Ok(T data, string message = "") => new(true, message, data);

    public static new OperationResult<T> Fail(string message) => new(false, message, default);
}
