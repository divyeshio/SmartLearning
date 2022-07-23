using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartLearning.Core.Entities.ClassroomAggregate;

namespace SmartLearning.Infrastructure.Data.Config;
public class ClassProposalConfiguration : IEntityTypeConfiguration<ClassProposal>
{
    public void Configure(EntityTypeBuilder<ClassProposal> builder)
    {

    }
}
