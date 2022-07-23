using Ardalis.Specification;

namespace SmartLearning.Core.Entities.BoardAggregate.Specifications;

public class CheckIfBoardExistsSpec : Specification<Board>, ISingleResultSpecification
{
    public CheckIfBoardExistsSpec(int boardId)
    {
        Query
            .Where(board => board.Id == boardId);

    }
}
