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
    var entity = await _repository.GetBySpecAsync(spec);
    if (entity == null) return NotFound();

    var response = new GetClassroomByIdResponse
    (
        id: entity.Id,
        name: entity.Name
    );
    return Ok(response);
  }
}
