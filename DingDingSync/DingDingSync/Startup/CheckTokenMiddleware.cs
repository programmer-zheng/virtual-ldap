using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace DingDingSync.Web.Startup
{
    public class CheckTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public CheckTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
        {
            var token = context.Request.Headers["token"];
            var path = context.Request.Path.ToString();
            if (path.StartsWith("/dingding"))
            {
                await _next(context);
            }
            else
            {
                //if (token.ToString().Equals(configuration.GetValue<string>("LdapRequestToken")))
                //{
                await _next(context);
                //}
                //else
                //{
                //    context.Response.StatusCode = 402;
                //}
            }
        }
    }
}
