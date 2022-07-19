using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.Core.Constants;
using SmartLearning.Core.Entities.BoardAggregate;
using SmartLearning.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartLearning.Web.Endpoints.BoardEndpoints;

public class Add : EndpointBaseAsync
    .WithRequest<AddBoardRequest>
    .WithActionResult<AddBoardResponse>
{
  private readonly IRepository<Board> _repository;

  public Add(IRepository<Board> repository)
  {
    _repository = repository;
  }

  [HttpPost(ApiRoutes.Boards.Add)]
  [SwaggerOperation(
      Summary = "Adds a new Board",
      Description = "Adds a new Board",
      OperationId = "Boards.Add",
      Tags = new[] { "BoardsEndpoints" })
  ]
  public override async Task<ActionResult<AddBoardResponse>> HandleAsync(AddBoardRequest request,
      CancellationToken cancellationToken)
  {
    if (request.Name == null || request.AbbrName == null)
    {
      return BadRequest();
    }

    var newBoard = new Board(request.AbbrName, request.Name);

    var createdItem = await _repository.AddAsync(newBoard); // TODO: pass cancellation token

    var response = new AddBoardResponse
    (
        id: createdItem.Id,
        name: createdItem.Name,
        abbrName: createdItem.AbbrName
    );

    return Ok(response);
  }
}
