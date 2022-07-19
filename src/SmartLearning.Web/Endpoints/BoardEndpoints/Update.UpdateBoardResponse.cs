namespace SmartLearning.Web.Endpoints.BoardEndpoints;

public class UpdateBoardResponse
{
  public UpdateBoardResponse(BoardRecord board)
  {
    Board = board;
  }
  public BoardRecord Board { get; set; }
}
