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
    var board = await _repository.FirstOrDefaultAsync(spec);
    if (board == null) return NotFound();

    var response = new GetBoardByIdResponse
    (
        id: board.Id,
        name: board.Name,
        abbrName: board.AbbrName,
        users: board.Users.Select(item => new UsersRecord(item.Id, item.FullName)).ToList()
    );
    return Ok(response);
  }
}
