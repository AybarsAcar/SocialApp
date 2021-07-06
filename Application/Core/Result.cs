namespace Application.Core
{
  /// <summary>
  /// Result will be returned from the API Controllers instead of entities or DTO's
  /// This result object will contain the response information as well
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class Result<T>
  {
    public bool IsSuccess { get; set; }
    public T Value { get; set; }
    public string Error { get; set; }

    public static Result<T> Success(T value) => new Result<T> {IsSuccess = true, Value = value};

    public static Result<T> Failure(string error) => new Result<T> {IsSuccess = false, Error = error};
  }
}