using Microsoft.Extensions.DependencyInjection;

namespace NCoreCoder.TestProject
{
    internal static class ServiceScopeExtension
    {
        public static TService GetRequriedService<TService>(this IServiceScope serviceScope)
        {
            return serviceScope.ServiceProvider.GetRequiredService<TService>();
        }
    }
}
