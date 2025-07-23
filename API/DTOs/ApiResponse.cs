using System.Collections.Generic;

namespace API.DTOs
{
    public class ApiResponse
    {
        public ApiResponse(int statusCode,
            string title = null,
            string message = null,
            string details = null,
            bool isHtmlEnabled = false,
            bool displayByDefault = false,
            bool showWithToastr = false,
            IEnumerable<string> errors = null,
            object data = null)
        {
            StatusCode = statusCode;
            Title = title ?? GetDefaultTitle(statusCode);
            Message = message ?? GetDefaultMessage(statusCode);
            Details = details;
            Errors = errors;
            IsHtmlEnabled = isHtmlEnabled;
            DisplayByDefault = displayByDefault;
            ShowWithToastr = showWithToastr;
            Data = data;
        }
        public int StatusCode { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public bool IsHtmlEnabled { get; set; }
        public bool DisplayByDefault { get; set; }
        public bool ShowWithToastr { get; set; }
        public object Data { get; set; }
        public IEnumerable<string> Errors { get; set; }

        private string GetDefaultTitle(int statusCode)
        {
            return statusCode switch
            {
                200 => "Success",
                201 => "Success",
                400 => "Bad Request",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Not Found",
                _ => "Error"
            };
        }
        private string GetDefaultMessage(int statusCode)
        {
            return statusCode switch
            {
                200 => "Your request has been successfully completed.",
                201 => "Your request has been successfully completed.",
                400 => "You have made a bad request.",
                401 => "You are not authorized.",
                403 => "Forbidden: Please refresh the page and try again. If the issue persists, contact our support team.",
                404 => "The requested resource was not found.",
                500 => "Somthing unexpected went wrong. Please contact the site administration.",
                _ => null
            };
        }
    }
}
