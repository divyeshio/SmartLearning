using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.Core.Entities.BoardAggregate;
using SmartLearning.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartLearning.Web.Endpoints.BoardEndpoints;

public class Delete : EndpointBaseAsync
    .WithRequest<DeleteBoardRequest>
    .WithoutResult
{
    private readonly IRepository<Board> _repository;

    public Delete(IRepository<Board> repository)
    {
        _repository = repository;
    }

    [HttpDelete(DeleteBoardRequest.Route)]
    [SwaggerOperation(
        Summary = "Deletes a Board",
        Description = "Deletes a Board",
        OperationId = "Boards.Delete",
        Tags = new[] { "BoardEndpoints" })
    ]
    public override async Task<ActionResult> HandleAsync([FromRoute] DeleteBoardRequest request,
        CancellationToken cancellationToken)
    {
        var aggregateToDelete = await _repository.GetByIdAsync(request.BoardId);
        if (aggregateToDelete == null) return NotFound();

        await _repository.DeleteAsync(aggregateToDelete);

        return NoContent();
    }
}
