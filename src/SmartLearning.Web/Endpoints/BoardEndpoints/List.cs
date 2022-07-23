using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.Core.Entities.BoardAggregate;
using SmartLearning.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartLearning.Web.Endpoints.BoardEndpoints;

public class List : EndpointBaseAsync
    .WithoutRequest
    .WithActionResult<BoardListResponse>
{
    private readonly IReadRepository<Board> _repository;

    public List(IReadRepository<Board> repository)
    {
        _repository = repository;
    }

    [HttpGet("/boards")]
    [SwaggerOperation(
        Summary = "Gets a list of all Boards",
        Description = "Gets a list of all Boards",
        OperationId = "Board.List",
        Tags = new[] { "BoardEndpoints" })
    ]
    public override async Task<ActionResult<BoardListResponse>> HandleAsync(CancellationToken cancellationToken)
    {
        var response = new BoardListResponse();
        response.Boards = (await _repository.ListAsync())
            .Select(board => new BoardRecord(board.Id, board.AbbrName, board.Name))
            .ToList();

        return Ok(response);
    }
}
