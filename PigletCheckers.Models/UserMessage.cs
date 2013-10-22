namespace PigletCheckers.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class UserMessage
    {
        [Key, Column("UserMessage_Id")]
        public int Id { get; set; }

        public string Text { get; set; }

        public virtual User User { get; set; }

        public virtual MessageState State { get; set; }

        public virtual UserMessageType Type { get; set; }

        public virtual Game Game { get; set; }
    }
}
