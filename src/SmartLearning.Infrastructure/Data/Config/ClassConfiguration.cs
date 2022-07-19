using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartLearning.Core.Entities.ClassAggregate;

namespace SmartLearning.Infrastructure.Data.Config;
public class ClassConfiguration : IEntityTypeConfiguration<Class>
{
  public void Configure(EntityTypeBuilder<Class> builder)
  {
    builder.HasOne(g => g.Board).WithMany().OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(g => g.Standard).WithMany().OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(g => g.Subject).WithMany().OnDelete(DeleteBehavior.Cascade);
  }
}
