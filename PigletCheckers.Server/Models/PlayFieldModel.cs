namespace PigletCheckers.Server.Models
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract(Name = "PlayField")]
    public class PlayFieldModel
    {
        [DataMember(Name = "gameId")]
        public int GameId { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "whitePieces")]
        public IEnumerable<PieceModel> WhitePieces { get; set; }

        [DataMember(Name = "blackPieces")]
        public IEnumerable<PieceModel> BlackPieces { get; set; }

        [DataMember(Name = "turn")]
        public long Turn { get; set; }

        [DataMember(Name = "inTurn")]
        public string PlayerInTurn { get; set; }
    }
}