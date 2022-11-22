using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartLearning.Core.Entities.BoardAggregate;
using SmartLearning.Core.Entities.TestAggregate;

namespace SmartLearning.Infrastructure.Data.Config;
public class TestConfiguration : IEntityTypeConfiguration<Test>
{
  public void Configure(EntityTypeBuilder<Test> builder)
  {
    builder.HasOne(s => s.CreatedBy).WithMany().OnDelete(DeleteBehavior.NoAction);
  }
}
