using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using VirtualLdap.Core.Configuration;

namespace VirtualLdap.Web.Test;

public static class HostingEnvironmentExtensions
{
    public static IConfigurationRoot GetAppConfiguration(this IWebHostEnvironment env)
    {
        return AppConfigurations.Get(env.ContentRootPath, env.EnvironmentName);
    }
}