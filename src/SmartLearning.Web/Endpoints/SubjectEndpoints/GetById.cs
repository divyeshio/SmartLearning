using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.Core.Entities.SubjectAggregate;
using SmartLearning.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartLearning.Web.Endpoints.SubjectEndpoints;

public class GetById : EndpointBaseAsync
    .WithRequest<GetSubjectByIdRequest>
    .WithActionResult<GetSubjectByIdResponse>
{
  private readonly IRepository<Subject> _repository;

  public GetById(IRepository<Subject> repository)
  {
    _repository = repository;
  }

  [HttpGet(GetSubjectByIdRequest.Route)]
  [SwaggerOperation(
      Summary = "Gets a single Subject",
      Description = "Gets a single Subject by Id",
      OperationId = "Subjects.GetById",
      Tags = new[] { "SubjectEndpoints" })
  ]
  public override async Task<ActionResult<GetSubjectByIdResponse>> HandleAsync([FromRoute] GetSubjectByIdRequest request,
      CancellationToken cancellationToken)
  {
    var entity = await _repository.GetByIdAsync(request.SubjectId);
    if (entity == null) return NotFound();

    var response = new GetSubjectByIdResponse
    (
        id: entity.Id,
        name: entity.Name
    );
    return Ok(response);
  }
}
