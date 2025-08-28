using System;

namespace demo_api.Exceptions
{
    public class CustomException : Exception
    {
        public string Detail { get; }
        public int StatusCode { get; }

        public CustomException(string message,  int statusCode,string detail= "") : base(message)
        {
            Detail = detail;
            StatusCode = statusCode;
        }
    }
}
