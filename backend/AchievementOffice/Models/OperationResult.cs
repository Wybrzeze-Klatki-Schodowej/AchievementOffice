namespace AchievementOffice.Models;

public class OperationResult<T>
{
    public bool IsSuccessful { get; set; }

    public string? ErrorMessage { get; set; }

    public T? Data { get; set; }

    public static OperationResult<T> Success(T data)
    {
        return new OperationResult<T>
        {
            IsSuccessful = true,
            Data = data
        };
    }

    public static OperationResult<T> Failure(string error)
    {
        return new OperationResult<T>
        {
            IsSuccessful = false,
            ErrorMessage = error
        };
    }
}

public class OperationResult
{
    public bool IsSuccessful { get; set; }

    public string? ErrorMessage { get; set; }

    public static OperationResult Success()
    {
        return new OperationResult
        {
            IsSuccessful = true
        };
    }

    public static OperationResult Failure(string error)
    {
        return new OperationResult
        {
            IsSuccessful = false,
            ErrorMessage = error
        };
    }
}