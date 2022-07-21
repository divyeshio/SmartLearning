using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SmartLearning.Infrastructure.Data;

namespace SmartLearning.IntegrationTests.Data;

public abstract class BaseEfRepoTestFixture
{
  protected ApplicationDbContext _dbContext;

  protected BaseEfRepoTestFixture()
  {
    var options = CreateNewContextOptions();
    var mockMediator = new Mock<IDomainEventDispatcher>();

    _dbContext = new ApplicationDbContext(options, mockMediator.Object);
  }

  protected static DbContextOptions<ApplicationDbContext> CreateNewContextOptions()
  {
    // Create a fresh service provider, and therefore a fresh
    // InMemory database instance.
    var serviceProvider = new ServiceCollection()
        .AddEntityFrameworkInMemoryDatabase()
        .BuildServiceProvider();

    // Create a new options instance telling the context to use an
    // InMemory database and the new service provider.
    var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
    builder.UseInMemoryDatabase("cleanarchitecture")
           .UseInternalServiceProvider(serviceProvider);

    return builder.Options;
  }

  /*protected EfRepository<Project> GetRepository()
  {
    return new EfRepository<Project>(_dbContext);
  }*/
}
