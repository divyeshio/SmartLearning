using Ardalis.ListStartupServices;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Serilog;
using SmartLearning.Core;
using SmartLearning.Core.Interfaces;
using SmartLearning.Infrastructure;
using SmartLearning.Infrastructure.Data;
using SmartLearning.Web;
using SmartLearning.Web.DTO;
using SmartLearning.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddDbContext(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddControllersWithViews(op =>
{
  op.Filters.Add(new ProducesAttribute("application/json"));
}).AddRazorRuntimeCompilation();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Smart Learning API", Version = "v1" });
    c.EnableAnnotations();
});

// add list services for diagnostic purposes - see https://github.com/ardalis/AspNetCoreStartupServices
builder.Services.Configure<ServiceConfig>(config =>
{
    config.Services = new List<ServiceDescriptor>(builder.Services);

    // optional - default path to view services is /listallservices - recommended to choose your own path
    config.Path = "/listservices";
});

builder.Services.ConfigureIdentity();

builder.Services.AddAuthentication();
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

builder.Services.AddAuthorization(options =>
{
    /*options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();*/
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "SmartLearning";
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/NotAuthorized";
});


builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule(new DefaultCoreModule());
    containerBuilder.RegisterModule(new DefaultInfrastructureModule(builder.Environment.EnvironmentName == "Development"));
});
builder.Services.AddTransient<IEmailSender, AuthMessageSender>();
builder.Services.AddSignalR(e =>
{
    e.MaximumReceiveMessageSize = 1024000;
    e.EnableDetailedErrors = true;
});

builder.Services.AddAutoMapper(typeof(WebMarker).Assembly);
builder.Services.AddSingleton<List<StudentViewModel>>();
builder.Services.AddSingleton<List<FacultyViewModel>>();
builder.Services.AddSingleton<List<User>>();

//builder.Logging.AddAzureWebAppDiagnostics(); add this if deploying to Azure

var app = builder.Build();

if (app.Environment.IsDevelopment())
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
                     Path.Combine(app.Environment.ContentRootPath, "StaticFiles")),
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
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
        context.Database.EnsureCreated();
        //SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
    }
}

app.Run();
