using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartLearning.Core.Entities.BoardAggregate;
using SmartLearning.Core.Entities.TestAggregate;

namespace SmartLearning.Infrastructure.Data.Config;
public class TestAttemptConfiguration : IEntityTypeConfiguration<TestAttempt>
{
  public void Configure(EntityTypeBuilder<TestAttempt> builder)
  {
    builder.HasOne(s => s.Student).WithMany().OnDelete(DeleteBehavior.SetNull);
  }
}
