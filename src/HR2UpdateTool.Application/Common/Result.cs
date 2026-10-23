using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR2UpdateTool.Application.Common
{
    public abstract class Result<T>
    {
        protected Result() { }

        public static Result<T> Success(T data) => new Success<T>(data);
        public static Result<T> Error(string message, Exception? exception = null) => new Error<T>(message, exception);
    }

    public sealed class Success<T> : Result<T>
    {
        public T Data { get; }

        public Success(T data)
        {
            Data = data;
        }
    }

    public sealed class Error<T> : Result<T>
    {
        public string ErrorMessage { get; }
        public Exception? Exception { get; }

        public Error(string message, Exception? exception = null)
        {
            ErrorMessage = message;
            Exception = exception;
        }
    }
}
