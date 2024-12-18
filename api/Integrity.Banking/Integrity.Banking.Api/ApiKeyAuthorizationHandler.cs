using Integrity.Banking.Domain.Models.Config;
using Microsoft.AspNetCore.Authorization;

namespace Integrity.Banking.Api
{
    public class ApiKeyAuthorizationHandler : AuthorizationHandler<ApiKeyRequirement>
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly AuthzConfig _authzConfig;
        public ApiKeyAuthorizationHandler(IHttpContextAccessor contextAccessor, AuthzConfig authzConfig) 
        {
            _contextAccessor = contextAccessor;
            _authzConfig = authzConfig;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiKeyRequirement requirement)
        {
            var httpContext = _contextAccessor.HttpContext;
            
            if (httpContext != null)
            {
                httpContext.Request.Headers.TryGetValue("x-api-key", out var apiKeyStringValues);

                var apiKey = apiKeyStringValues.FirstOrDefault();

                if (!string.IsNullOrEmpty(apiKey) && apiKey == _authzConfig.ApiKey)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
