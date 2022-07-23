using Ardalis.Specification;

namespace SmartLearning.Core.Entities.ClassroomAggregate.Specifications;

public class ClassroomByIdWithUsersSpec : Specification<Classroom>, ISingleResultSpecification
{

    public ClassroomByIdWithUsersSpec(int classroomId)
    {
        Query
          .Where(classroom => classroom.Id == classroomId)
                .Include(project => project.Users);
    }
}
