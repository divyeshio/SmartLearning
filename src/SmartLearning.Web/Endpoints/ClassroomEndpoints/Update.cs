using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.Core.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartLearning.Web.Endpoints.ClassroomEndpoints;

public class Update : EndpointBaseAsync
    .WithRequest<UpdateClassroomRequest>
    .WithActionResult<UpdateClassroomResponse>
{
    private readonly IClassroomService _classroomService;

    public Update(IClassroomService classroomService)
    {
        _classroomService = classroomService;
    }

    [HttpPut(UpdateClassroomRequest.Route)]
    [SwaggerOperation(
        Summary = "Updates a Classroom",
        Description = "Updates a Classroom with a longer description",
        OperationId = "Classrooms.Update",
        Tags = new[] { "ClassroomEndpoints" })
    ]
    public override async Task<ActionResult<UpdateClassroomResponse>> HandleAsync(UpdateClassroomRequest request,
        CancellationToken cancellationToken)
    {
        var existingClassroom = await _classroomService.GetByIdAsync(request.Id);

        if (existingClassroom == null)
        {
            return NotFound();
        }
        existingClassroom = await _classroomService.UpdateClassroom(existingClassroom, request.BoardId, request.StandardId, request.SubjectId);


        var response = new UpdateClassroomResponse(
            classroom: new ClassroomRecord(existingClassroom.Id, existingClassroom.Name)
        );
        return Ok(response);
    }
}
