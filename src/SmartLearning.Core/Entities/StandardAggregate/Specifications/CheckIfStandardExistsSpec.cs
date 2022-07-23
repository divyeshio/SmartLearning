using Ardalis.Specification;
using SmartLearning.Core.Entities.StandardAggregate;

namespace SmartLearning.Core.Entities.BoardAggregate.Specifications;

public class CheckIfStandardExistsSpec : Specification<Standard>, ISingleResultSpecification
{
    public CheckIfStandardExistsSpec(int standardId)
    {
        Query
            .Where(standard => standard.Id == standardId);

    }
}
