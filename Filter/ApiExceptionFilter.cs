using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InvoicingSystem.Filter
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        private readonly IHostEnvironment _hostEnvironment;

        public ApiExceptionFilter(IHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public void OnException(ExceptionContext context)
        {
            int statusCode = 500;
            string? details = null;
            string message = "حدث خطأ غير متوقع";

            if (_hostEnvironment.IsDevelopment())
            {
                details = context.Exception.ToString();
            }

            if(context.Exception is ArgumentNullException)
            {
                statusCode = 400;
                message = "المدخلات غير صالحة: " + context.Exception.Message;
            }
            else if(context.Exception is KeyNotFoundException)
            {
                statusCode = 404;
                message = "المورد المطلوب غير موجود: " + context.Exception.Message;
            }
            else if (context.Exception is UnauthorizedAccessException)
            {
                statusCode = 401;
                message = "غير مصرح لك بالوصول إلى هذا المورد: " + context.Exception.Message;
            }
            else if (context.Exception is InvalidOperationException)
            {
                statusCode = 409;
                message = "عملية غير صالحة: " + context.Exception.Message;
            }
            else if (context.Exception is Exception)
            {
                statusCode = 500;
                message = "حدث خطأ غير متوقع: " + context.Exception.Message;
            }
            else
            {
                details = context.Exception.Message;
            }

            context.Result = new JsonResult(new
            {
                Success = false,
                Message = message,
                Details = details
            })
            {
                StatusCode = statusCode
            };
            context.ExceptionHandled = true;
        }
    }
}