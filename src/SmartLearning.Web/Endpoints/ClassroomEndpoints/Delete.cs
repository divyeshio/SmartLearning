using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.Core.Entities.ClassroomAggregate;
using SmartLearning.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartLearning.Web.Endpoints.ClassroomEndpoints;

public class Delete : EndpointBaseAsync
    .WithRequest<DeleteClassroomRequest>
    .WithoutResult
{
    private readonly IRepository<Classroom> _repository;

    public Delete(IRepository<Classroom> repository)
    {
        _repository = repository;
    }

    [HttpDelete(DeleteClassroomRequest.Route)]
    [SwaggerOperation(
        Summary = "Deletes a Classroom",
        Description = "Deletes a Classroom",
        OperationId = "Classrooms.Delete",
        Tags = new[] { "ClassroomEndpoints" })
    ]
    public override async Task<ActionResult> HandleAsync([FromRoute] DeleteClassroomRequest request,
        CancellationToken cancellationToken)
    {
        var aggregateToDelete = await _repository.GetByIdAsync(request.ClassroomId);
        if (aggregateToDelete == null) return NotFound();

        await _repository.DeleteAsync(aggregateToDelete);

        return NoContent();
    }
}
