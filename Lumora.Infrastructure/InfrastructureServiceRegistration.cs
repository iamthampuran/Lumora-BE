using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Lumora.Infrastructure.Data;
using Lumora.Infrastructure.Repositories;
using Lumora.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace Lumora.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        //connection to db
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        //other services registration
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddHttpContextAccessor();
        services.AddMinio(configureClient => configureClient
          .WithEndpoint(configuration["Minio:Endpoint"])
          .WithCredentials(configuration["Minio:AccessKey"], configuration["Minio:SecretKey"])
          .WithSSL(configuration.GetValue<bool>("Minio:UseSSL"))
          .Build());

        //application services registration
        services.AddScoped<IMinioService, MinioService>();

        //repository registration
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IStudioRepository, StudioRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();  
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IInquiryRepository, InquiryRepository>();
        services.AddScoped<IEventTypeRepository, EventTypeRepository>();
        services.AddScoped<ITagRepository, TagRepository>();

        return services;

    }
}
