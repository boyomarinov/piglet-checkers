namespace PigletCheckers.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Game
    {
        public Game()
        {
            this.Pieces = new HashSet<Piece>();
        }

        [Key, Column("Game_Id")]
        public int Id { get; set; }

        public string Title { get; set; }

        public string Password { get; set; }

        public virtual User BlackUser { get; set; }
        
        public virtual User WhiteUser { get; set; }

        public virtual GameState State { get; set; }

        public virtual ICollection<Piece> Pieces { get; set; }

        public virtual User UserInTurn { get; set; }

        public int Turn { get; set; }
    }
}
