namespace PigletCheckers.Server.Models
{
    using System.Runtime.Serialization;

    [DataContract(Name = "Game")]
    public class GameModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }
    }
}