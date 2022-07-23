using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.Core.Entities.SubjectAggregate;
using SmartLearning.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartLearning.Web.Endpoints.SubjectEndpoints;

public class Delete : EndpointBaseAsync
    .WithRequest<DeleteSubjectRequest>
    .WithoutResult
{
    private readonly IRepository<Subject> _repository;

    public Delete(IRepository<Subject> repository)
    {
        _repository = repository;
    }

    [HttpDelete(DeleteSubjectRequest.Route)]
    [SwaggerOperation(
        Summary = "Deletes a Subject",
        Description = "Deletes a Subject",
        OperationId = "Subjects.Delete",
        Tags = new[] { "SubjectEndpoints" })
    ]
    public override async Task<ActionResult> HandleAsync([FromRoute] DeleteSubjectRequest request,
        CancellationToken cancellationToken)
    {
        var aggregateToDelete = await _repository.GetByIdAsync(request.SubjectId);
        if (aggregateToDelete == null) return NotFound();

        await _repository.DeleteAsync(aggregateToDelete);

        return NoContent();
    }
}
