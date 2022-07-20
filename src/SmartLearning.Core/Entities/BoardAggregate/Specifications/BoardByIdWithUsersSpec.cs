using Ardalis.Specification;

namespace SmartLearning.Core.Entities.BoardAggregate.Specifications;

public class BoardByIdWithUsersSpec : Specification<Board>, ISingleResultSpecification
{
    public BoardByIdWithUsersSpec(int boardId)
    {
        Query
            .Where(board => board.Id == boardId)
            .Include(board => board.Users);
    }
}
