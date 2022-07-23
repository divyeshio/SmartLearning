using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.Core.Entities.StandardAggregate;
using SmartLearning.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartLearning.Web.Endpoints.StandardEndpoints;

public class Delete : EndpointBaseAsync
    .WithRequest<DeleteStandardRequest>
    .WithoutResult
{
    private readonly IRepository<Standard> _repository;

    public Delete(IRepository<Standard> repository)
    {
        _repository = repository;
    }

    [HttpDelete(DeleteStandardRequest.Route)]
    [SwaggerOperation(
        Summary = "Deletes a Standard",
        Description = "Deletes a Standard",
        OperationId = "Standards.Delete",
        Tags = new[] { "StandardEndpoints" })
    ]
    public override async Task<ActionResult> HandleAsync([FromRoute] DeleteStandardRequest request,
        CancellationToken cancellationToken)
    {
        var aggregateToDelete = await _repository.GetByIdAsync(request.StandardId);
        if (aggregateToDelete == null) return NotFound();

        await _repository.DeleteAsync(aggregateToDelete);

        return NoContent();
    }
}
