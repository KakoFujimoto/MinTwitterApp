using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MinTwitterApp.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "未処理の例外が発生しました");

        context.Result = new ObjectResult(new
        {
            error = "サーバーエラーが発生しました。"
        })
        {
            StatusCode = 500
        };

        context.ExceptionHandled = true;
    }

}