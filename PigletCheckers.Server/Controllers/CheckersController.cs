namespace PigletCheckers.Server.Controllers
{
    using System.Net.Http;
    using System.Web.Http;
    using PigletCheckers.Server.Models;
    using PigletCheckers.Server.Persisters;

    public class CheckersController : BaseApiController
    {
        [HttpPost]
        [ActionName("move")]
        public HttpResponseMessage PerformMove(string sessionKey, int gameId, [FromBody] MoveModel move)
        {
            var responseMsg = this.PerformOperation(() =>
            {
                var userId = UserDataPersister.LoginUser(sessionKey);
                CheckersDataPersister.PerformMove(userId, gameId, move.PieceId, move.Position.X, move.Position.Y);
            });

            return responseMsg;
        }
    }
}