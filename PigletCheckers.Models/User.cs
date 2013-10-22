namespace PigletCheckers.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class User
    {
        public User()
        {
            this.Games = new HashSet<Game>();
            this.Pieces = new HashSet<Piece>();
            this.UserMessages = new HashSet<UserMessage>();
        }

        [Key, Column("User_Id")]
        public int Id { get; set; }

        public string Username { get; set; }

        public string Nickname { get; set; }

        public string AuthCode { get; set; }

        public string SessionKey { get; set; }

        public long Score { get; set; }

        public virtual ICollection<Game> Games { get; set; }

        public virtual ICollection<Piece> Pieces { get; set; }

        public virtual ICollection<UserMessage> UserMessages { get; set; }
    }
}
