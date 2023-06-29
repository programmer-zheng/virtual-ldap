using Abp.Collections.Extensions;
using Abp.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace VirtualLdap.Web.Startup;

public class CheckTokenFilterAttribute : IAuthorizationFilter
{
    private readonly IConfiguration _configuration;

    public CheckTokenFilterAttribute(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var requestToken = context.HttpContext.Request.Headers["token"];
        if (requestToken.IsNullOrEmpty())
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var ldapRequestToken = _configuration["LdapRequestToken"];

        if (!ldapRequestToken.IsNullOrWhiteSpace() &&
            !requestToken.ToString().Equals(ldapRequestToken, StringComparison.OrdinalIgnoreCase))
        {
            context.Result = new UnauthorizedResult();
            return;
        }
    }
}