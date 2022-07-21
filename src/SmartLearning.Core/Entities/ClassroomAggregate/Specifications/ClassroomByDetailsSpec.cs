using Ardalis.Specification;

namespace SmartLearning.Core.Entities.ClassroomAggregate.Specifications;

public class ClassroomByDetailsSpec : Specification<Classroom>, ISingleResultSpecification
{
    public ClassroomByDetailsSpec(int boardId, int standardId, int subjectId)
    {
        Query
            .Where(project => project.Id == boardId)
            .Include(project => project.Users);
    }
}
