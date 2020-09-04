using Microsoft.AspNetCore.Mvc.Filters;

namespace LocalChachaAdminApi.Middlewares
{
    public class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
        }
    }
}