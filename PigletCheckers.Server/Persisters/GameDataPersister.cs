namespace PigletCheckers.Server.Persisters
{
    using System.Collections.Generic;
    using System.Linq;
    using PigletCheckers.Data;
    using PigletCheckers.Models;
    using PigletCheckers.Server.Models;

    public class GameDataPersister : BaseDataPersister
    {
        private const int MinTitleLength = 6;
        private const int MaxTitleLength = 40;
        private const string ValidTitleChars = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM_1234567890 -";

        private const int PiecesCount = 8;

        private static readonly PositionModel[] PieceCoordinates = 
        {
            new PositionModel() { X = 1, Y = 0 }, new PositionModel() { X = 3, Y = 0 }, new PositionModel() { X = 5, Y = 0 }, new PositionModel() { X = 7, Y = 0 },
            new PositionModel() { X = 0, Y = 1 }, new PositionModel() { X = 2, Y = 1 }, new PositionModel() { X = 4, Y = 1 }, new PositionModel() { X = 6, Y = 1 },
            
            new PositionModel() { X = 1, Y = 6 }, new PositionModel() { X = 3, Y = 6 }, new PositionModel() { X = 5, Y = 6 }, new PositionModel() { X = 7, Y = 6 },
            new PositionModel() { X = 0, Y = 7 }, new PositionModel() { X = 2, Y = 7 }, new PositionModel() { X = 4, Y = 7 }, new PositionModel() { X = 6, Y = 7 },
        };        

        public static void CreateGame(int userId, string title, string password)
        {
            GameDataPersister.ValidateGameTitle(title);
            if (password != null)
            {
                GameDataPersister.ValidateGamePassword(password);
            }

            var context = new GameDbContext();
            using (context)
            {
                var user = GetUser(userId, context);

                var game = new Game()
                {
                    Title = title,
                    Password = password,
                    State = GameState.Open,
                    WhiteUser = user
                };

                context.Games.Add(game);

                context.SaveChanges();

                user.Games.Add(game);

                context.SaveChanges();
            }
        }

        public static void JoinGame(int userId, int gameId, string password)
        {
            if (password != null)
            {
                GameDataPersister.ValidateGamePassword(password);
            }

            var context = new GameDbContext();
            using (context)
            {
                var user = GetUser(userId, context);

                var game = context.Games.Find(gameId);
                if (game == null)
                {
                    throw new ServerErrorException("No such game", "ERR_INV_GAME");
                }

                BaseDataPersister.ValidateOpenGameStatus(game, context);

                if (game.Password != null && password != null && game.Password != password)
                {
                    throw new ServerErrorException("Invalid game password", "INV_GAME_PASS");
                }

                game.BlackUser = user;

                string messageText = string.Format("{0} just joined your game {1}", game.WhiteUser.Nickname, game.Title);
                BaseDataPersister.SendMessage(messageText, game.WhiteUser, game, UserMessageType.GameJoined, context);

                game.State = GameState.Full;

                user.Games.Add(game);

                context.SaveChanges();
            }
        }

        public static void StartGame(int gameId)
        {
            var context = new GameDbContext();
            using (context)
            {
                var game = BaseDataPersister.GetGame(gameId, context);
                if (game.State != GameState.Full)
                {
                    throw new ServerErrorException("Nobody has joined this game", "INV_GAME_STATE");
                }

                GameDataPersister.InitialisePieces(game, context);

                game.State = GameState.InProgress;

                game.Turn = 0;
                var userInTurn = ((Rand.Next() & 1) == 0) ? game.WhiteUser : game.BlackUser;
                game.UserInTurn = userInTurn;

                var gameStartedMessageText = string.Format("{0} just started game {1}", game.WhiteUser.Nickname, game.Title);
                BaseDataPersister.SendMessage(gameStartedMessageText, game.BlackUser, game, UserMessageType.GameStarted, context);

                var gameMoveMessageText = string.Format("It is your turn in game {0}", game.Title);
                BaseDataPersister.SendMessage(gameMoveMessageText, userInTurn, game, UserMessageType.GameMove, context);

                context.SaveChanges();
            }
        }

        public static IEnumerable<OpenGameModel> GetOpenGames(int userId)
        {
            var context = new GameDbContext();
            using (context)
            {
                var user = GetUser(userId, context);
                IEnumerable<OpenGameModel> openGames;

                var allOpenGames = context.Games.Where(game => game.State == GameState.Open).ToList();
                if (allOpenGames.Any())
                {
                    openGames = allOpenGames.Where(game => game.WhiteUser.Id != user.Id).Select(OpenGameModel.GetOpenGameModel);
                }
                else
                {
                    openGames = new List<OpenGameModel>();
                }

                return openGames.ToList();
            }
        }

        public static IEnumerable<ActiveGameModel> GetActiveGames(int userId)
        {
            var context = new GameDbContext();
            using (context)
            {
                var user = BaseDataPersister.GetUser(userId, context);
                IEnumerable<ActiveGameModel> activeGameModels;

                var activeGames =
                    context.Games.Where(game => game.State == GameState.InProgress || game.State == GameState.Full)
                        .ToList();
                if (activeGames.Any())
                {      
                    activeGameModels = activeGames.Where(game => game.BlackUser.Id == user.Id || game.WhiteUser.Id == user.Id).Select(ActiveGameModel.GetActiveGameModel);
                }
                else
                {
                    activeGameModels = new List<ActiveGameModel>();
                }
                
                return activeGameModels.ToList();
            }
        }

        public static PlayFieldModel GetPlayField(int gameId)
        {
            var context = new GameDbContext();
            using (context)
            {
                Game game = BaseDataPersister.GetGame(gameId, context);
                BaseDataPersister.ValidateGameInProgressStatus(game, context);

                PlayFieldModel playField = new PlayFieldModel();

                playField.GameId = game.Id;
                playField.WhitePieces =
                    game.Pieces.Where(piece => piece.Owner.Id == game.WhiteUser.Id)
                        .Select(PieceModel.GetPieceModel)
                        .ToList();
                playField.BlackPieces =
                    game.Pieces.Where(piece => piece.Owner.Id == game.BlackUser.Id)
                        .Select(PieceModel.GetPieceModel)
                        .ToList();
                playField.Title = game.Title;
                playField.Turn = game.Turn;
                playField.PlayerInTurn = game.UserInTurn.Nickname;

                return playField;
            }
        }

        private static void InitialisePieces(Game game, GameDbContext context)
        {
            for (int index = 0; index < PiecesCount; index++)
            {
                Piece piece = new Piece() { Game = game, Owner = game.BlackUser, XCoordinate = PieceCoordinates[index].X, YCoordinate = PieceCoordinates[index].Y };
                context.Pieces.Add(piece);
                context.SaveChanges();
                game.Pieces.Add(piece);
                game.BlackUser.Pieces.Add(piece);
            }

            for (int index = PiecesCount; index < PieceCoordinates.Length; index++)
            {
                Piece piece = new Piece() { Game = game, Owner = game.BlackUser, XCoordinate = PieceCoordinates[index].X, YCoordinate = PieceCoordinates[index].Y };
                context.Pieces.Add(piece);
                context.SaveChanges();
                game.Pieces.Add(piece);
                game.WhiteUser.Pieces.Add(piece);
            }

            context.SaveChanges();
        }

        private static void ValidateGameTitle(string title)
        {
            if (title == null || title.Length < MinTitleLength || title.Length > MaxTitleLength)
            {
                throw new ServerErrorException(string.Format("The title of the game should be between {0} and {1} characters", MinTitleLength, MaxTitleLength), "INV_TITLE_LEN");
            }
            else if (title.Any(character => !ValidTitleChars.Contains(character)))
            {
                throw new ServerErrorException("The title of the game contains invalid characters", "INV_TITLE_CHARS");
            }
        }

        private static void ValidateGamePassword(string authCode)
        {
            if (authCode.Length != GameDataPersister.Sha1CodeLength)
            {
                throw new ServerErrorException("Invalid game password", "INV_GAME_PASS");
            }
        }
    }
}