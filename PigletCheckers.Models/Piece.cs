namespace PigletCheckers.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Piece
    {
        [Key, Column("Piece_Id")]
        public int Id { get; set; }

        public int XCoordinate { get; set; }

        public int YCoordinate { get; set; }

        public virtual User Owner { get; set; }

        public virtual Game Game { get; set; }
    }
}
