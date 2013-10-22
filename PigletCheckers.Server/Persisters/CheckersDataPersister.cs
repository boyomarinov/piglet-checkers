namespace PigletCheckers.Server.Persisters
{
    using System;
    using System.Linq;
    using PigletCheckers.Data;
    using PigletCheckers.Models;
    using PigletCheckers.Server.Models;

    public class CheckersDataPersister : BaseDataPersister
    {
        protected const int PlayFieldRows = 8;
        protected const int PlayFieldColumns = 8;

        private const int OptimalTurnsToWin = 34;
        private const int MaxPossibleScorePerWin = 100;

        private const string GameMoveMessageText = "{0} made their move in game {1}";
        private const string GameWonMessageText = "You won in game {0} against {1} in {2} moves";
        private const string GameLostMessageText = "You lost in game {0} against {1} in {2} moves";

        private static readonly PositionModel[] AllowedPieceMoveVectors = 
        { 
            new PositionModel() { X = 1, Y = -1 }, new PositionModel() { X = -1, Y = -1 }, 
            new PositionModel() { X = 1, Y = 1 }, new PositionModel() { X = -1, Y = 1 } 
        };
                  
        public static void PerformMove(int userId, int gameId, int pieceId, int toXCoordinate, int toYCoordinate)
        {
            var context = new GameDbContext();
            using (context)
            {
                Game game = GetGame(gameId, context);
                BaseDataPersister.ValidateGameInProgressStatus(game, context);

                User user = GetUser(userId, context);
                Piece piece = GetPiece(pieceId, context);

                CheckersDataPersister.ValidateUserPieceInGame(user, piece, game);

                CheckersDataPersister.ValidatePieceMovePosition(game, piece, toXCoordinate, toYCoordinate);

                piece.XCoordinate = toXCoordinate;
                piece.YCoordinate = toYCoordinate;

                var otherUser = (game.WhiteUser.Id == user.Id) ? game.BlackUser : game.WhiteUser;

                game.Turn++;

                var messageText = string.Format(GameMoveMessageText, user.Nickname, game.Title);
                var gameMoveUserMessagesType = UserMessageType.GameMove;
                BaseDataPersister.SendMessage(messageText, otherUser, game, gameMoveUserMessagesType, context);

                bool whiteWins = !game.Pieces.Where(gamePiece => gamePiece.Owner.Id == game.WhiteUser.Id).Any(whitePiece => whitePiece.YCoordinate > 1);
                bool blackWins = !game.Pieces.Where(gamePiece => gamePiece.Owner.Id == game.BlackUser.Id).Any(blackPiece => blackPiece.YCoordinate < 6);

                if (whiteWins || blackWins)
                {
                    game.State = GameState.Finished;

                    int unmodifiedScore = MaxPossibleScorePerWin - (game.Turn - OptimalTurnsToWin);

                    int score = unmodifiedScore > 0 ? unmodifiedScore << 10 : 0;

                    user.Score += score;

                    UserMessageType gameFinished = UserMessageType.GameFinished;

                    var gameWonMessage = string.Format(GameWonMessageText, game.Title, otherUser.Nickname, game.Turn);
                    BaseDataPersister.SendMessage(gameWonMessage, user, game, gameFinished, context);

                    var gameLostMessage = string.Format(GameLostMessageText, game.Title, user.Nickname, game.Turn);
                    BaseDataPersister.SendMessage(gameLostMessage, otherUser, game, gameFinished, context);
                }
                else
                {
                    game.UserInTurn = otherUser;
                }
                
                context.SaveChanges();
            }
        }

        private static Piece GetPiece(int pieceId, GameDbContext context)
        {
            var piece = context.Pieces.Find(pieceId);
            if (piece == null)
            {
                throw new ServerErrorException("No such piece", "ERR_INV_PIECE");
            }

            return piece;
        }

        private static void ValidatePieceMovePosition(Game game, Piece piece, int toXCoordinate, int toYCoordinate)
        {
            if (toXCoordinate < 0 || toXCoordinate >= PlayFieldRows || toYCoordinate < 0 || toYCoordinate >= PlayFieldColumns)
            {
                throw new ServerErrorException("Pieces cannot go outside of the playfield", "INV_PIECE_POS");
            }

            if (game.Pieces.Any(gamePiece => gamePiece.XCoordinate == toXCoordinate && gamePiece.YCoordinate == toYCoordinate))
            {
                throw new ServerErrorException("Position is already occupied", "INV_PIECE_POS");
            }

            PositionModel moveVector = new PositionModel() { X = toXCoordinate - piece.XCoordinate, Y = toYCoordinate - piece.YCoordinate };

            if (Math.Abs(moveVector.X) != Math.Abs(moveVector.Y) || CheckersDataPersister.AllowedPieceMoveVectors.Any(vector => vector.X == moveVector.X && vector.Y == moveVector.Y))
            {
                throw new ServerErrorException("Piece cannot move to that position", "INV_PIECE_POS");
            }

            moveVector.X /= Math.Abs(moveVector.X);
            moveVector.Y /= Math.Abs(moveVector.Y);

            PositionModel currentPosition = new PositionModel() { X = piece.XCoordinate, Y = piece.YCoordinate };

            do
            {
                if (!game.Pieces.Any(gamePiece => gamePiece.XCoordinate == currentPosition.X && gamePiece.YCoordinate == currentPosition.Y))
                {
                    throw new ServerErrorException("You have to make a jump", "INV_PIECE_POS");
                }

                currentPosition.X += moveVector.X;
                currentPosition.Y += moveVector.Y;
            } 
            while (currentPosition.X != toXCoordinate && currentPosition.Y != toYCoordinate);
        }

        private static void ValidateUserPieceInGame(User user, Piece piece, Game game)
        {
            if (game.UserInTurn != user)
            {
                throw new ServerErrorException("It is not your turn", "INV_USR_TURN");
            }

            if (!user.Pieces.Any(userPiece => userPiece.Id == piece.Id))
            {
                throw new ServerErrorException("This is not your piece", "INV_USR_PIECE");
            }

            if (!game.Pieces.Any(gamePiece => gamePiece.Id == piece.Id))
            {
                throw new ServerErrorException("No such piece in the game", "INV_USR_GAME");
            }
        }
    }
}