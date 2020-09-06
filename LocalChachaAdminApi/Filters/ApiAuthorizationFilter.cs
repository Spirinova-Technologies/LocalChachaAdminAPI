using LocalChachaAdminApi.Core.Interfaces;
using LocalChachaAdminApi.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LocalChachaAdminApi.Filters
{
    public class ApiAuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly ITokenService tokenService;
        public ApiAuthorizationFilter(ITokenService tokenService)
        {
            this.tokenService = tokenService;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // Allow Anonymous skips all authorization. Works only for .net core 2.2
            if (context.Filters.Any(item => item is IAllowAnonymousFilter))
            {
                return;
            }

            // Allow Anonymous skips all authorization. Works for .net core 3.0
            ControllerActionDescriptor controllerActionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;
            object allowAnonymousMethodAttribute = controllerActionDescriptor?.MethodInfo?
                .GetCustomAttributes(typeof(AllowAnonymousAttribute), true).FirstOrDefault();
            object allowAnonymousControllerAttribute = controllerActionDescriptor?.ControllerTypeInfo?
                .GetCustomAttributes(typeof(AllowAnonymousAttribute), true).FirstOrDefault();

            if (allowAnonymousMethodAttribute != null || allowAnonymousControllerAttribute != null)
            {
                return;
            }

            IHeaderDictionary headerDictionary = context.HttpContext.Request.Headers;
            // SystemConstants.ApiAuthorizationHeaderName Used only for Swagger >= 5.X. https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1295
            string headerValue = headerDictionary.GetHeaderValue("Api-" + HeaderNames.Authorization);
            StringValues authorizationHeaderValues = new StringValues();
            if (!string.IsNullOrWhiteSpace(headerValue))
            {
                authorizationHeaderValues = headerDictionary["Api-" + HeaderNames.Authorization];
            }
            else
            {
                // Keep support for Authorization header for tools which send it.
                headerValue = headerDictionary.GetHeaderValue(HeaderNames.Authorization);
                if (!string.IsNullOrWhiteSpace(headerValue))
                {
                    authorizationHeaderValues = headerDictionary[HeaderNames.Authorization];
                }
            }

            if (!string.IsNullOrWhiteSpace(headerValue))
            {
                AuthorizeWithAuthorizationHeader(context, authorizationHeaderValues);
            }
            else
            {
                context.Result = new UnauthorizedObjectResult("Unauthorized access!");
                return;
            }
        }

        private void AuthorizeWithAuthorizationHeader(AuthorizationFilterContext context, StringValues authorizationHeaderValues)
        {
            string[] headerValuesSplit = authorizationHeaderValues.ToString().Split(" ", StringSplitOptions.RemoveEmptyEntries);
            if (headerValuesSplit.Length != 2)
            {
                context.Result = new UnauthorizedObjectResult("Unauthorized access!");
                return;
            }

            string authTokenSchemeConfigured = "Bearer";

            string authTokenScheme = headerValuesSplit[0];
            var token = headerValuesSplit[1];

            if (!string.Equals(authTokenSchemeConfigured, authTokenScheme, StringComparison.InvariantCultureIgnoreCase))
            {
                context.Result = new UnauthorizedObjectResult("Unauthorized access!");
                return;
            }

            if (string.IsNullOrWhiteSpace(token) || !tokenService.IsTokenValid(token, out _))
            {
                context.Result = new UnauthorizedObjectResult("Unauthorized access!");
                return;
            }
        }
    }
}