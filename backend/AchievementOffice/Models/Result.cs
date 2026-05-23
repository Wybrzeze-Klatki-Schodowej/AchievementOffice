namespace AchievementOffice.Models;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Value { get; private set; }
    public string? ErrorMessage { get; private set; }

    private Result() { }

    public static Result<T> Success(T value)
    {
        return new Result<T> 
        { 
            IsSuccess = true, 
            Value = value 
        };
    }

    public static Result<T> Fail(string errorMessage)
    {
        return new Result<T> 
        { 
            IsSuccess = false, 
            ErrorMessage = errorMessage 
        };
    }
}

public class Result
{
    public bool IsSuccess { get; private set; }

    public string? ErrorMessage { get; private set; }

    private Result() {}

    public static Result Success()
    {
        return new Result
        {
            IsSuccess = true
        };
    }

    public static Result Fail(string errorMessage)
    {
        return new Result
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}