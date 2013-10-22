namespace PigletCheckers.Server.Persisters
{
    using System;
    using System.Linq;
    using PigletCheckers.Data;
    using PigletCheckers.Models;
    using PigletCheckers.Server.Models;

    public class BaseDataPersister
    {
        protected const int Sha1CodeLength = 40;

        protected const string GameStatusOpen = "open";
        protected const string GameStatusFull = "full";
        protected const string GameStatusInProgress = "in-progress";
        protected const string GameStatusFinished = "finished";

        protected const string MessageStateUnread = "unread";

        protected const string UserMessageTypeGameStarted = "game-started";
        protected const string UserMessageTypeGameJoined = "game-joined";
        protected const string UserMessageTypeGameFinished = "game-finished";
        protected const string UserMessageTypeGameMove = "game-move";

        protected static Random Rand = new Random();

        protected static User GetUser(int userId, GameDbContext context)
        {
            var user = context.Users.Find(userId);
            if (user == null)
            {
                throw new ServerErrorException("Invalid user", "ERR_INV_USR");
            }

            return user;
        }

        protected static Game GetGame(int gameId, GameDbContext context)
        {
            var game = context.Games.Find(gameId);
            if (game == null)
            {
                throw new ServerErrorException("No such game", "ERR_INV_GAME");
            }

            return game;
        }

        protected static void SendMessage(string text, User toUser, Game game, UserMessageType msgType, GameDbContext context)
        {
            toUser.UserMessages.Add(new UserMessage()
            {
                Game = game,
                State = MessageState.Unread,
                Type = msgType,
                Text = text,
            });
        }

        protected static void ValidateOpenGameStatus(Game game, GameDbContext context)
        {
            if (game.State == GameState.InProgress)
            {
                throw new ServerErrorException("Game has already been started", "INV_GAME_STATE");
            }
            else if (game.State == GameState.Finished)
            {
                throw new ServerErrorException("Game is already finished", "INV_GAME_STATE");
            }
            else if (game.State == GameState.Full)
            {
                throw new ServerErrorException("Game is not started yet", "INV_GAME_STATE");
            }
        }

        protected static void ValidateGameInProgressStatus(Game game, GameDbContext context)
        {
            if (game.State == GameState.Open)
            {
                throw new ServerErrorException("Game not yet full", "INV_GAME_STATE");
            }
            else if (game.State == GameState.Finished)
            {
                throw new ServerErrorException("Game is already finished", "INV_GAME_STATE");
            }
            else if (game.State == GameState.Full)
            {
                throw new ServerErrorException("Game is not started yet", "INV_GAME_STATE");
            }
        }
    }
}