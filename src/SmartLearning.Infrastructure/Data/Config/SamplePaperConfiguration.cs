using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartLearning.Core.Entities.BoardAggregate;
using SmartLearning.Core.Entities.ClassroomAggregate;

namespace SmartLearning.Infrastructure.Data.Config;
public class SamplePaperConfiguration : IEntityTypeConfiguration<SamplePaper>
{
  public void Configure(EntityTypeBuilder<SamplePaper> builder)
  {
    builder.HasOne(s => s.UploadedBy).WithMany().OnDelete(DeleteBehavior.NoAction);
  }
}
