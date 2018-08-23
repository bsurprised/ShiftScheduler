using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Onyx.ShiftScheduler.Core.Interfaces;

namespace Onyx.ShiftScheduler.Api.Exceptions
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;

        public ExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;

            var code = (int) HttpStatusCode.InternalServerError;
            if (exception is IException ex)
                code = (int) ex.StatusCode;

            response.ContentType = "application/json";
            response.StatusCode = code;

            var message = new
            {
                // customize if needed
                error = exception.Message,
                statusCode = code,
                /*/ dev env
                stack = exception.StackTrace,
                innerException = new
                {
                    error = exception.InnerException?.Message,
                    stack = exception.InnerException?.StackTrace
                }  // end dev env*/
            };

            await response.WriteAsync(JsonConvert.SerializeObject(message));
        }
    }
}