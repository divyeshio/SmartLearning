using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.Core.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartLearning.Web.Endpoints.ClassroomEndpoints;

public class Add : EndpointBaseAsync
    .WithRequest<AddClassroomRequest>
    .WithActionResult<AddClassroomResponse>
{
  private readonly IClassroomService _classroomService;

  public Add(IClassroomService classroomService)
  {
    _classroomService = classroomService;
  }

  [HttpPost(AddClassroomRequest.Route)]
  [SwaggerOperation(
      Summary = "Adds a new Classroom",
      Description = "Adds a new Classroom",
      OperationId = "Classrooms.Add",
      Tags = new[] { "ClassroomEndpoints" })
  ]
  public override async Task<ActionResult<AddClassroomResponse>> HandleAsync(AddClassroomRequest request,
  CancellationToken cancellationToken)
  {

    var newClassroom = await _classroomService.CreateClassroomAsync(request.BoardId, request.StandardId, request.SubjectId);

    var response = new AddClassroomResponse
    (
        id: newClassroom.Id,
        name: newClassroom.Name
    );

    return Ok(response);
  }
}
