using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.Core.ProjectAggregate;
using SmartLearning.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartLearning.Web.Endpoints.ProjectEndpoints;

public class Update : EndpointBaseAsync
    .WithRequest<UpdateProjectRequest>
    .WithActionResult<UpdateProjectResponse>
{
  private readonly IRepository<Project> _repository;

  public Update(IRepository<Project> repository)
  {
    _repository = repository;
  }

  [HttpPut(UpdateProjectRequest.Route)]
  [SwaggerOperation(
      Summary = "Updates a Project",
      Description = "Updates a Project with a longer description",
      OperationId = "Projects.Update",
      Tags = new[] { "ProjectEndpoints" })
  ]
  public override async Task<ActionResult<UpdateProjectResponse>> HandleAsync(UpdateProjectRequest request,
      CancellationToken cancellationToken)
  {
    if (request.Name == null)
    {
      return BadRequest();
    }
    var existingProject = await _repository.GetByIdAsync(request.Id);

    if (existingProject == null)
    {
      return NotFound();
    }
    existingProject.UpdateName(request.Name);

    await _repository.UpdateAsync(existingProject);

    var response = new UpdateProjectResponse(
        project: new ProjectRecord(existingProject.Id, existingProject.Name)
    );
    return Ok(response);
  }
}
