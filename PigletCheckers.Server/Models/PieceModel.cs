namespace PigletCheckers.Server.Models
{
    using System.Runtime.Serialization;
    using PigletCheckers.Models;

    [DataContract(Name = "Piece")]
    public class PieceModel
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "owner")]
        public string Owner { get; set; }

        [DataMember(Name = "position")]
        public PositionModel Position { get; set; }

        public static PieceModel GetPieceModel(Piece piece)
        {
            PieceModel pieceModel = new PieceModel
            {
                Id = piece.Id,
                Owner = piece.Owner.Nickname,
                Position = new PositionModel()
                {
                    X = piece.XCoordinate,
                    Y = piece.YCoordinate
                }
            };

            return pieceModel;
        }
    }
}