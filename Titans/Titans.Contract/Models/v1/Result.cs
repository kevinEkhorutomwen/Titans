namespace Titans.Contract;
using Titans.Contract.Models.v1;

public class Result<T>
{
    public T? Data { get; }
    public Error? Error { get; }

    private Result(T data)
    {
        Data = data;
    }

    private Result(Error error)
    {
        Error = error;
    }

    public static Result<T> SetOk(T data)
    {
        return new Result<T>(data);
    }

    public static Result<T> SetError(Error error)
    {
        return new Result<T>(error);
    }
}
public class Result
{
    public Error? Error { get; }
    private Result(Error error)
    {
        Error = error;
    }

    public Result() { }
    public static Result SetError(Error error)
    {
        return new Result(error);
    }
}