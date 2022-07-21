using Ardalis.Specification;
using SmartLearning.Core.Entities.SubjectAggregate;

namespace SmartLearning.Core.Entities.SubjectAggregate.Specifications;

public class SubjectByIdWithUsersSpec : Specification<Subject>, ISingleResultSpecification
{
  public SubjectByIdWithUsersSpec(int subjectId)
  {
    Query
        .Where(subject => subject.Id == subjectId);
  }
}
