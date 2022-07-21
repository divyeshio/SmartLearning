using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartLearning.Core.Entities.BoardAggregate;

namespace SmartLearning.Infrastructure.Data.Config;
public class BoardConfiguration : IEntityTypeConfiguration<Board>
{
  public void Configure(EntityTypeBuilder<Board> builder)
  {
    builder.Property(p => p.AbbrName)
        .HasMaxLength(10)
        .IsRequired();

    builder.Property(p => p.Name)
      .HasMaxLength(100)
        .IsRequired();
  }
}
