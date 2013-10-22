namespace PigletCheckers.Server.Models
{
    using System.Runtime.Serialization;

    [DataContract(Name = "Position")]
    public class PositionModel
    {
        [DataMember(Name = "x")]
        public int X { get; set; }

        [DataMember(Name = "y")]
        public int Y { get; set; }
    }
}