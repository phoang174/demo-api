using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Results
{
    public class Result
    {
        public bool IsSuccess { get; init; }
        public bool IsFailure => !IsSuccess;
        public Error? Error { get; init; }

        protected Result(bool isSuccess, Error? error)
        {
            if (isSuccess && error != null)
                throw new InvalidOperationException("Success result cannot have an error.");
            if (!isSuccess && error == null)
                throw new InvalidOperationException("Failure result must have an error.");

            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Ok() => new Result(true, null);
        public static Result Fail(Error error) => new Result(false, error);
    }

    public class Result<T> : Result
    {
        public T? Value { get; init; }

        protected internal Result(T? value, bool isSuccess, Error? error)
            : base(isSuccess, error)
        {
            Value = value;
        }

        public static Result<T> Ok(T value) => new Result<T>(value, true, null);
        public static new Result<T> Fail(Error error) => new Result<T>(default, false, error);
    }
    public record Error(
    string Message,
    string Detail ,
    int Code
);

}
