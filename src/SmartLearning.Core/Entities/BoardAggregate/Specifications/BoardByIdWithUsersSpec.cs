using Ardalis.Specification;

namespace SmartLearning.Core.Entities.BoardAggregate.Specifications;

public class BoardByIdWithUsersSpec : Specification<Board>, ISingleResultSpecification
{
    public BoardByIdWithUsersSpec(int projectId)
    {
        Query
            .Where(project => project.Id == projectId)
            .Include(project => project.Users);
    }
}
