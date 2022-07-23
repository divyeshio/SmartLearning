using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartLearning.Core.Entities.ClassroomAggregate;

namespace SmartLearning.Infrastructure.Data.Config;
public class ClassConfiguration : IEntityTypeConfiguration<Classroom>
{
    public void Configure(EntityTypeBuilder<Classroom> builder)
    {
        builder.HasOne(g => g.Board).WithMany().OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(g => g.Standard).WithMany().OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(g => g.Subject).WithMany().OnDelete(DeleteBehavior.Cascade);
    }
}
