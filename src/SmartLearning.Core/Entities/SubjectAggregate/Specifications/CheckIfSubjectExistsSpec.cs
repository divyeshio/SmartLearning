using Ardalis.Specification;
using SmartLearning.Core.Entities.SubjectAggregate;

namespace SmartLearning.Core.Entities.BoardAggregate.Specifications;

public class CheckIfSubjectExistsSpec : Specification<Subject>, ISingleResultSpecification
{
    public CheckIfSubjectExistsSpec(int subjectId)
    {
        Query
            .Where(subject => subject.Id == subjectId);

    }
}
