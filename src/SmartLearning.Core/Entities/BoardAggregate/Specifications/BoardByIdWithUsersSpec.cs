using Ardalis.Specification;
using SmartLearning.Core.Entities.BoardAggregate;

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
