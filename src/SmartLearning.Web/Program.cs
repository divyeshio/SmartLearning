using Autofac.Extensions.DependencyInjection;
using Serilog;
using SmartLearning.Web;

public class Program
{
  public static void Main(string[] args)
  {
    CreateHostBuilder(args).Build().Run();
  }

  public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
            .UseSerilog((context, _, config) => config.ReadFrom.Configuration(context.Configuration))
          .ConfigureWebHostDefaults(webBuilder =>
          {
            webBuilder.UseStartup<Startup>();
          }).UseServiceProviderFactory(new AutofacServiceProviderFactory());
}
