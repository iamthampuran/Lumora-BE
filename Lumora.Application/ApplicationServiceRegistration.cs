using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Wolverine;
using Wolverine.FluentValidation;

namespace Lumora.Application;

public static class ApplicationServiceRegistration
{
    public static void AddApplicationServices(this IHostBuilder builder, IConfiguration configuration)
    {
        // Register application services here
        // Example: hostBuilder.ConfigureServices(services => services.AddTransient<IMyService, MyService>());
        builder.UseWolverine(options =>
        {
            options.UseFluentValidation();
            options.Discovery.IncludeAssembly(typeof(ApplicationServiceRegistration).Assembly);
        });

    }
}
