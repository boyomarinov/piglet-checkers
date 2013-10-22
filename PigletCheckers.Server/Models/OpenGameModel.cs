namespace PigletCheckers.Server.Models
{
    using System.Runtime.Serialization;
    using PigletCheckers.Models;

    [DataContract(Name = "Game")]
    public class OpenGameModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "creator")]
        public string Creator { get; set; }

        public static OpenGameModel GetOpenGameModel(Game game)
        {
            OpenGameModel openGameModel = new OpenGameModel
            {
                Id = game.Id,
                Title = game.Title,
                Creator = game.WhiteUser.Nickname
            };

            return openGameModel;
        }
    }
}