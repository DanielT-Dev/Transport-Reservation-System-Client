using System.Collections.Generic;

namespace MPP_Client.Models
{
    public class Reservation : IIdentifiable<int?>
    {
        public int?         Id     { get; set; }
        public int          UserId { get; set; }
        public int          RaceId { get; set; }
        public string       Name   { get; set; }
        public List<int>    Seats  { get; set; }

        public Reservation(int? id, int userId, int raceId, string name, List<int> seats)
        {
            Id     = id;
            UserId = userId;
            RaceId = raceId;
            Name   = name;
            Seats  = seats;
        }

        public Reservation(int userId, int raceId, string name, List<int> seats)
            : this(null, userId, raceId, name, seats) { }

        public override string ToString() =>
            $"{Id} {Name} {UserId} {RaceId} [{string.Join(", ", Seats)}]";
    }
}
