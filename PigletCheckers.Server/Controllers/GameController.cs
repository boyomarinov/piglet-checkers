namespace PigletCheckers.Server.Controllers
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Web.Http;
    using PigletCheckers.Server.Models;
    using PigletCheckers.Server.Persisters;

    public class GameController : BaseApiController
    {
        [HttpPost]
        [ActionName("create")]
        public HttpResponseMessage CreateGame(string sessionKey, [FromBody] GameModel gameModel)
        {
            var responseMsg = this.PerformOperation(() =>
            {
                var userId = UserDataPersister.LoginUser(sessionKey);
                GameDataPersister.CreateGame(userId, gameModel.Title, gameModel.Password);
            });

            return responseMsg;
        }

        [HttpPost]
        [ActionName("join")]
        public HttpResponseMessage JoinGame(string sessionKey, [FromBody] GameModel gameModel)
        {
            var responseMsg = this.PerformOperation(() =>
            {
                var userId = UserDataPersister.LoginUser(sessionKey);
                GameDataPersister.JoinGame(userId, gameModel.Id, gameModel.Password);
            });

            return responseMsg;
        }

        [HttpGet]
        [ActionName("start")]
        public HttpResponseMessage StartGame(string sessionKey, int gameId)
        {
            var responseMsg = this.PerformOperation(() =>
            {
                UserDataPersister.LoginUser(sessionKey);
                GameDataPersister.StartGame(gameId);
            });

            return responseMsg;
        }

        [HttpGet]
        [ActionName("open")]
        public HttpResponseMessage GetOpenGames(string sessionKey)
        {
            var responseMsg = this.PerformOperation(() =>
            {
                var userId = UserDataPersister.LoginUser(sessionKey);
                var games = GameDataPersister.GetOpenGames(userId);

                return games;
            });

            return responseMsg;
        }

        [HttpGet]
        [ActionName("my-active")]
        public HttpResponseMessage GetActiveGames(string sessionKey)
        {
            var responseMsg = this.PerformOperation(() =>
            {
                var userId = UserDataPersister.LoginUser(sessionKey);
                IEnumerable<OpenGameModel> games = GameDataPersister.GetActiveGames(userId);

                return games;
            });

            return responseMsg;
        }

        [HttpGet]
        [ActionName("field")]
        public HttpResponseMessage GetPlayField(string sessionKey, int gameId)
        {
            var responseMsg = this.PerformOperation(() =>
            {
                UserDataPersister.LoginUser(sessionKey);
                PlayFieldModel playField = GameDataPersister.GetPlayField(gameId);

                return playField;
            });

            return responseMsg;
        }

        [HttpGet]
        [ActionName("scores")]
        public HttpResponseMessage GetAllUsers(string sessionKey)
        {
            var responseMsg = this.PerformOperation(() =>
            {
                UserDataPersister.LoginUser(sessionKey);
                IEnumerable<UserScoreModel> userScores = UserDataPersister.GetAllUserScores();

                return userScores;
            });

            return responseMsg;
        }
    }
}