using Ardalis.Specification.EntityFrameworkCore;
using SmartLearning.SharedKernel.Interfaces;

namespace SmartLearning.Infrastructure.Data;

// inherit from Ardalis.Specification type
public class EfRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T> where T : class, IAggregateRoot
{
  public EfRepository(ApplicationDbContext dbContext) : base(dbContext)
  {
  }
}
