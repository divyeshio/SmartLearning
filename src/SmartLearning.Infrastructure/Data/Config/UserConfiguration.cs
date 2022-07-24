using Microsoft.AspNetCore.Identity;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartLearning.Core.Entities;
using SmartLearning.Core.Entities.BoardAggregate;
using SmartLearning.Core.Entities.UsersAggregate;
using SmartLearning.Core.Entities.Common;

namespace SmartLearning.Infrastructure.Data.Config;
public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
  public void Configure(EntityTypeBuilder<ApplicationUser> builder)
  {
    builder.HasOne(b => b.Board).WithMany(b => b.Users).OnDelete(DeleteBehavior.NoAction);
    builder.HasOne(b => b.Standard).WithMany().OnDelete(DeleteBehavior.NoAction);
    builder.HasOne(b => b.Subject).WithMany().OnDelete(DeleteBehavior.NoAction);


    builder.HasMany(u => u.Classes).WithMany(g => g.Users)
        .UsingEntity<UserClass>(ug => ug
                                  .HasOne(ug => ug.Class)
                                  .WithMany()
                                  .HasForeignKey("ClassId").OnDelete(DeleteBehavior.NoAction),
                                ug => ug
                                  .HasOne(ug => ug.User)
                                  .WithMany()
                                  .HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade))
                               .ToTable("UserClass")
                               .HasKey(ug => new { ug.ClassId, ug.UserId });

    builder.Property(b => b.isEnabled).HasDefaultValue(true);
    builder.HasOne(u => u.FaceData).WithOne(f => f.User).HasForeignKey<FaceData>(f => f.UserId);

    builder.ToTable(name: "Users");
  }
}
