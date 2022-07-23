using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.Core.Entities.SubjectAggregate;
using SmartLearning.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartLearning.Web.Endpoints.SubjectEndpoints;

public class List : EndpointBaseAsync
    .WithoutRequest
    .WithActionResult<SubjectListResponse>
{
    private readonly IReadRepository<Subject> _repository;

    public List(IReadRepository<Subject> repository)
    {
        _repository = repository;
    }

    [HttpGet("/subjects")]
    [SwaggerOperation(
        Summary = "Gets a list of all Subjects",
        Description = "Gets a list of all Subjects",
        OperationId = "Subject.List",
        Tags = new[] { "SubjectEndpoints" })
    ]
    public override async Task<ActionResult<SubjectListResponse>> HandleAsync(CancellationToken cancellationToken)
    {
        var response = new SubjectListResponse();
        response.Subjects = (await _repository.ListAsync())
            .Select(subject => new SubjectRecord(subject.Id, subject.Name))
            .ToList();

        return Ok(response);
    }
}
