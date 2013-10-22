namespace PigletCheckers.Server.Models
{
    using System.Runtime.Serialization;
    using PigletCheckers.Models;

    [DataContract(Name = "Message")]
    public class MessageModel
    {
        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "gameId")]
        public int GameId { get; set; }

        [DataMember(Name = "gameTitle")]
        public string GameTitle { get; set; }

        [DataMember(Name = "type")]
        public UserMessageType Type { get; set; }

        [DataMember(Name = "state")]
        public MessageState State { get; set; }
    }
}