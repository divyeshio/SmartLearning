using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.Core.Entities.SubjectAggregate;
using SmartLearning.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartLearning.Web.Endpoints.SubjectEndpoints;

public class Update : EndpointBaseAsync
    .WithRequest<UpdateSubjectRequest>
    .WithActionResult<UpdateSubjectResponse>
{
    private readonly IRepository<Subject> _repository;

    public Update(IRepository<Subject> repository)
    {
        _repository = repository;
    }

    [HttpPut(UpdateSubjectRequest.Route)]
    [SwaggerOperation(
        Summary = "Updates a Subject",
        Description = "Updates a Subject with a longer description",
        OperationId = "Subjects.Update",
        Tags = new[] { "SubjectEndpoints" })
    ]
    public override async Task<ActionResult<UpdateSubjectResponse>> HandleAsync(UpdateSubjectRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Name == null)
        {
            return BadRequest();
        }
        var existingSubject = await _repository.GetByIdAsync(request.Id);

        if (existingSubject == null)
        {
            return NotFound();
        }
        existingSubject.UpdateName(request.Name);

        await _repository.UpdateAsync(existingSubject);

        var response = new UpdateSubjectResponse(
            subject: new SubjectRecord(existingSubject.Id, existingSubject.Name)
        );
        return Ok(response);
    }
}
