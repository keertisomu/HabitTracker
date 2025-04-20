using HabitTracker.Core;
using HabitTracker.Core.Repository;
using HabitTracker.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class CoreServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IHabitTrackerRepository, HabitTrackerRepository>();
        services.AddScoped<IHabitTrackerService, HabitTrackerService>();
        services.AddScoped<IStreakCalculationService, StreakCalculationService>();
        return services;
    }

    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        string connectionString
    )
    {
        services.AddDbContext<HabitTrackerDbContext>(options =>
            options.UseNpgsql(connectionString)
        );
        return services;
    }
}
