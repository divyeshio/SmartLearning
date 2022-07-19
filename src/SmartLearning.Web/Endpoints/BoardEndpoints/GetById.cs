using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.Core.Entities.BoardAggregate;
using SmartLearning.Core.Entities.BoardAggregate.Specifications;
using SmartLearning.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartLearning.Web.Endpoints.BoardEndpoints;

public class GetById : EndpointBaseAsync
    .WithRequest<GetBoardByIdRequest>
    .WithActionResult<GetBoardByIdResponse>
{
  private readonly IRepository<Board> _repository;

  public GetById(IRepository<Board> repository)
  {
    _repository = repository;
  }

  [HttpGet(GetBoardByIdRequest.Route)]
  [SwaggerOperation(
      Summary = "Gets a single Board",
      Description = "Gets a single Board by Id",
      OperationId = "Boards.GetById",
      Tags = new[] { "BoardEndpoints" })
  ]
  public override async Task<ActionResult<GetBoardByIdResponse>> HandleAsync([FromRoute] GetBoardByIdRequest request,
      CancellationToken cancellationToken)
  {
    var spec = new BoardByIdWithUsersSpec(request.BoardId);
    var entity = await _repository.GetBySpecAsync(spec);
    if (entity == null) return NotFound();

    var response = new GetBoardByIdResponse
    (
        id: entity.Id,
        name: entity.Name,
        abbrName: entity.AbbrName,
        users: entity.Users.Select(item => new UsersRecord(item.Id, item.FullName)).ToList()
    );
    return Ok(response);
  }
}
