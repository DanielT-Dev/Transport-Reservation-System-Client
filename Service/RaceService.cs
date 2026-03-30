using System;
using System.Collections.Generic;
using System.Linq;
using MPP_Client.Models;
using MPP_Client.Repository;

namespace MPP_Client.Service
{
    public class RaceService : IService<Race, int>
    {
        private readonly RaceDAO _dao = new RaceDAO();

        public Race GetById(int id)
        {
            if (id <= 0) throw new ArgumentException("Id must be positive.");
            return _dao.GetById(id);
        }

        public List<Race> GetAll() => _dao.GetAll();

        public void Add(Race race)
        {
            if (string.IsNullOrWhiteSpace(race.Destination))
                throw new ArgumentException("Destination cannot be empty.");
            if (string.IsNullOrWhiteSpace(race.Date))
                throw new ArgumentException("Date cannot be empty.");
            if (string.IsNullOrWhiteSpace(race.Time))
                throw new ArgumentException("Time cannot be empty.");
            if (race.AvailableSeats == null || race.AvailableSeats.Count == 0)
                throw new ArgumentException("Race must have at least one seat.");

            _dao.Add(race);
        }

        public void Update(Race race)
        {
            if (race.Id == null) throw new ArgumentException("Race has no Id.");
            if (string.IsNullOrWhiteSpace(race.Destination))
                throw new ArgumentException("Destination cannot be empty.");

            _dao.Update(race);
        }

        public void Delete(int id)
        {
            if (id <= 0) throw new ArgumentException("Id must be positive.");
            _dao.Delete(id);
        }

        // Extra: how many seats are still free
        public int CountAvailableSeats(int raceId)
        {
            var race = _dao.GetById(raceId);
            return race?.AvailableSeats.Count(s => s) ?? 0;
        }
    }
}