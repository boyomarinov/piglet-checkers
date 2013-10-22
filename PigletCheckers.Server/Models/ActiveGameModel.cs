namespace PigletCheckers.Server.Models
{
    using System.Runtime.Serialization;
    using PigletCheckers.Models;

    [DataContract(Name = "Game")]
    public class ActiveGameModel : OpenGameModel
    {
        [DataMember(Name = "status")]
        public GameState State { get; set; }

        public static ActiveGameModel GetActiveGameModel(Game game)
        {
            ActiveGameModel activeGameModel = new ActiveGameModel
            {
                Id = game.Id,
                Creator = game.WhiteUser.Nickname,
                State = game.State,
                Title = game.Title
            };

            return activeGameModel;
        }
    }
}