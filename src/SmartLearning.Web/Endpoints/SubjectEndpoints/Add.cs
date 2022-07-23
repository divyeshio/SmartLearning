using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.Core.Entities.SubjectAggregate;
using SmartLearning.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartLearning.Web.Endpoints.SubjectEndpoints;

public class Add : EndpointBaseAsync
    .WithRequest<AddSubjectRequest>
    .WithActionResult<AddSubjectResponse>
{
    private readonly IRepository<Subject> _repository;

    public Add(IRepository<Subject> repository)
    {
        _repository = repository;
    }

    [HttpPost(AddSubjectRequest.Route)]
    [SwaggerOperation(
        Summary = "Adds a new Subject",
        Description = "Adds a new Subject",
        OperationId = "Subjects.Add",
        Tags = new[] { "SubjectEndpoints" })
    ]
    public override async Task<ActionResult<AddSubjectResponse>> HandleAsync(AddSubjectRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Name == null)
        {
            return BadRequest();
        }

        var newSubject = new Subject(request.Name);

        var createdItem = await _repository.AddAsync(newSubject);

        var response = new AddSubjectResponse
        (
            id: createdItem.Id,
            name: createdItem.Name
        );

        return Ok(response);
    }
}
