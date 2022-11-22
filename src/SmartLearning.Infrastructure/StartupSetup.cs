using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartLearning.Core.Entities.UsersAggregate;
using SmartLearning.Infrastructure.Data;

namespace SmartLearning.Infrastructure;

public static class StartupSetup
{
  public static void AddDbContext(this IServiceCollection services, string connectionString) =>
      services.AddDbContext<ApplicationDbContext>(options =>
         options.UseSqlServer(connectionString)
      //options.UseLoggerFactory(MyLoggerFactory).LogTo(Console.WriteLine, LogLevel.Debug).EnableSensitiveDataLogging();
      // will be created in web project root
      );

  /// <summary>
  /// Configures Asp.Net Identity with custom options
  /// </summary>
  /// <param name="services"></param>
  public static void ConfigureIdentity(this IServiceCollection services) =>
    services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
      options.SignIn.RequireConfirmedAccount = true;
      options.SignIn.RequireConfirmedEmail = true;
      options.User.RequireUniqueEmail = true;
    })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddClaimsPrincipalFactory<MyUserClaimsPrincipalFactory>()
                .AddDefaultTokenProviders();

}
