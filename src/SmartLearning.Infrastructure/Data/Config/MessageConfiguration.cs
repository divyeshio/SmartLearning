using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartLearning.Core.Entities.BoardAggregate;
using SmartLearning.Core.Entities.Common;

namespace SmartLearning.Infrastructure.Data.Config;
public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
  public void Configure(EntityTypeBuilder<Message> builder)
  {
    builder.HasOne(b => b.FromUser).WithMany(u => u.Messages).OnDelete(DeleteBehavior.NoAction);
  }
}
