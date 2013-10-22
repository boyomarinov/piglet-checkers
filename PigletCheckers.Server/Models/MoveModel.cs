namespace PigletCheckers.Server.Models
{
    using System.Runtime.Serialization;

    [DataContract(Name = "Move")]
    public class MoveModel
    {
        [DataMember(Name = "pieceId")]
        public int PieceId { get; set; }

        [DataMember(Name = "position")]
        public PositionModel Position { get; set; }
    }
}