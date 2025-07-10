using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MinTwitterApp.Filters;

public class UnauthorizedExceptionFilter : Attribute, IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is UnauthorizedAccessException)
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                error = context.Exception.Message
            });

            context.ExceptionHandled = true;
        }
    }
}