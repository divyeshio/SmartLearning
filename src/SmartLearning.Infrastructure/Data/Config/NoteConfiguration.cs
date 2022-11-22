using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartLearning.Core.Entities.BoardAggregate;
using SmartLearning.Core.Entities.ClassroomAggregate;

namespace SmartLearning.Infrastructure.Data.Config;
public class NoteConfiguration : IEntityTypeConfiguration<Note>
{
  public void Configure(EntityTypeBuilder<Note> builder)
  {
    builder.HasOne(s => s.UploadedBy).WithMany().OnDelete(DeleteBehavior.NoAction);
  }
}
