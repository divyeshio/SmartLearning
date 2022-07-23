using Ardalis.ListStartupServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using SmartLearning.Core.Interfaces;
using SmartLearning.Infrastructure;
using SmartLearning.Infrastructure.Data;
using SmartLearning.Web.DTO;
using SmartLearning.Web.Hubs;

namespace SmartLearning.Web;
public class Startup
{
  public Startup(IConfiguration configuration)
  {
    Configuration = configuration;
  }

  public IConfiguration Configuration { get; }

  public void ConfigureServices(IServiceCollection services)
  {
    services.Configure<CookiePolicyOptions>(options =>
    {
      options.CheckConsentNeeded = context => true;
      options.MinimumSameSitePolicy = SameSiteMode.None;
    });

    services.AddDbContext(Configuration.GetConnectionString("DefaultConnection"));
    services.AddDatabaseDeveloperPageExceptionFilter();

    services.AddControllersWithViews(op =>
    {
      op.Filters.Add(new ProducesAttribute("application/json"));
    }).AddRazorRuntimeCompilation();

    services.AddSwaggerGen(c =>
    {
      c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
      c.EnableAnnotations();
    });

    // add list services for diagnostic purposes - see https://github.com/ardalis/AspNetCoreStartupServices
    services.Configure<ServiceConfig>(config =>
    {
      config.Services = new List<ServiceDescriptor>(services);

      // optional - default path to view services is /listallservices - recommended to choose your own path
      config.Path = "/listservices";
    });

    services.ConfigureIdentity();

    services.AddAuthentication();
    /*.AddGoogle(options =>
    {
      options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
      options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
      options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
    })
    .AddFacebook(facebookOptions =>
    {
      facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"];
      facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
      facebookOptions.Fields.Add("picture");
    })
    .AddMicrosoftAccount(microsoftOptions =>
    {
      microsoftOptions.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"];
      microsoftOptions.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];
    });*/

    services.AddAuthorization(options =>
    {
      /*options.FallbackPolicy = new AuthorizationPolicyBuilder()
          .RequireAuthenticatedUser()
          .Build();*/
    });

    services.ConfigureApplicationCookie(options =>
    {
      options.Cookie.Name = "SmartLearning";
      options.LoginPath = "/Account/Login";
      options.AccessDeniedPath = "/Account/NotAuthorized";
    });


    /*builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
    {
      containerBuilder.RegisterModule(new DefaultCoreModule());
      containerBuilder.RegisterModule(new DefaultInfrastructureModule(builder.Environment.EnvironmentName == "Development"));
    });*/
    services.AddTransient<IEmailSender, AuthMessageSender>();
    services.AddSignalR(e =>
    {
      e.MaximumReceiveMessageSize = 1024000;
      e.EnableDetailedErrors = true;
    });

    services.AddAutoMapper(typeof(WebMarker).Assembly);
    services.AddSingleton<List<StudentViewModel>>();
    services.AddSingleton<List<FacultyViewModel>>();
    services.AddSingleton<List<User>>();
  }

  public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
  {
    if (env.IsDevelopment())
    {
      app.UseDeveloperExceptionPage();
      app.UseShowAllServicesMiddleware();
    }
    else
    {
      app.UseExceptionHandler("/Home/Error");
      app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseDefaultFiles();
    app.UseStaticFiles();
    app.UseRouting();

    // Enable middleware to serve generated Swagger as a JSON endpoint.
    app.UseSwagger();

    // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Smart Learning API V1"));

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseHttpsRedirection();
    app.UseStaticFiles(new StaticFileOptions
    {
      FileProvider = new PhysicalFileProvider(
                         Path.Combine(env.ContentRootPath, "StaticFiles")),
      RequestPath = "/StaticFiles",
    });
    app.UseCookiePolicy();




    app.UseEndpoints(endpoints =>
    {
      endpoints.MapDefaultControllerRoute();
      endpoints.MapHub<ChatHub>("/chatHub");
      endpoints.MapHub<LiveHub>("/LiveHub");
    });

    // Seed Database
    using (var scope = app.ApplicationServices.CreateScope())
    {
      var services = scope.ServiceProvider;

      try
      {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
        context.Database.EnsureCreated();
        SeedData.Initialize(services);
      }
      catch (Exception ex)
      {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
      }
    }
  }
}
