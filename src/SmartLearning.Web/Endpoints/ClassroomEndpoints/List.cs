using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.Core.Entities.ClassroomAggregate;
using SmartLearning.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartLearning.Web.Endpoints.ClassroomEndpoints;

public class List : EndpointBaseAsync
    .WithoutRequest
    .WithActionResult<ClassroomListResponse>
{
    private readonly IReadRepository<Classroom> _repository;

    public List(IReadRepository<Classroom> repository)
    {
        _repository = repository;
    }

    [HttpGet("/classrooms")]
    [SwaggerOperation(
        Summary = "Gets a list of all Classrooms",
        Description = "Gets a list of all Classrooms",
        OperationId = "Classroom.List",
        Tags = new[] { "ClassroomEndpoints" })
    ]
    public override async Task<ActionResult<ClassroomListResponse>> HandleAsync(CancellationToken cancellationToken)
    {
        var response = new ClassroomListResponse();
        response.Classrooms = (await _repository.ListAsync())
            .Select(classroom => new ClassroomRecord(classroom.Id, classroom.Name))
            .ToList();

        return Ok(response);
    }
}
