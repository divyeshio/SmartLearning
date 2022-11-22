using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.Core.Entities.ClassroomAggregate;
using SmartLearning.Core.Entities.ClassroomAggregate.Specifications;
using SmartLearning.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartLearning.Web.Endpoints.ClassroomEndpoints;

public class GetById : EndpointBaseAsync
    .WithRequest<GetClassroomByIdRequest>
    .WithActionResult<GetClassroomByIdResponse>
{
  private readonly IRepository<Classroom> _repository;

  public GetById(IRepository<Classroom> repository)
  {
    _repository = repository;
  }

  [HttpGet(GetClassroomByIdRequest.Route)]
  [SwaggerOperation(
      Summary = "Gets a single Classroom",
      Description = "Gets a single Classroom by Id",
      OperationId = "Classrooms.GetById",
      Tags = new[] { "ClassroomEndpoints" })
  ]
  public override async Task<ActionResult<GetClassroomByIdResponse>> HandleAsync([FromRoute] GetClassroomByIdRequest request,
      CancellationToken cancellationToken)
  {
    var spec = new ClassroomByIdWithUsersSpec(request.ClassroomId);
    var classroom = await _repository.FirstOrDefaultAsync(spec);
    if (classroom == null) return NotFound();

    var response = new GetClassroomByIdResponse
    (
        id: classroom.Id,
        name: classroom.Name
    );
    return Ok(response);
  }
}
