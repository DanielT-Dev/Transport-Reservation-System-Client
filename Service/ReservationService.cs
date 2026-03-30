using System;
using System.Collections.Generic;
using System.Linq;
using MPP_Client.Models;
using MPP_Client.Repository;

namespace MPP_Client.Service
{
    public class ReservationService : IService<Reservation, int>
    {
        private readonly ReservationDAO _dao        = new ReservationDAO();
        private readonly RaceDAO        _raceDao    = new RaceDAO();

        public ReservationService(ReservationDAO reservationDAO, RaceDAO raceDAO)
        {
            this._dao = reservationDAO;
            this._raceDao = raceDAO;
        }

        public Reservation GetById(int id)
        {
            if (id <= 0) throw new ArgumentException("Id must be positive.");
            return _dao.GetById(id);
        }

        public List<Reservation> GetAll() => _dao.GetAll();

        public void Add(Reservation reservation)
        {
            if (string.IsNullOrWhiteSpace(reservation.Name))
                throw new ArgumentException("Passenger name cannot be empty.");
            if (reservation.Seats == null || reservation.Seats.Count == 0)
                throw new ArgumentException("At least one seat must be selected.");

            // Validate that all requested seats are actually available
            var race = _raceDao.GetById(reservation.RaceId);
            if (race == null)
                throw new ArgumentException($"Race {reservation.RaceId} not found.");

            foreach (var seat in reservation.Seats)
            {
                if (seat < 0 || seat >= race.AvailableSeats.Count)
                    throw new ArgumentException($"Seat {seat} does not exist on this race.");
                if (!race.AvailableSeats[seat])
                    throw new ArgumentException($"Seat {seat} is already taken.");
            }

            _dao.Add(reservation);

            // Mark seats as taken in the race
            foreach (var seat in reservation.Seats)
                race.AvailableSeats[seat] = false;

            _raceDao.Update(race);
        }

        public void Update(Reservation reservation)
        {
            if (reservation.Id == null) throw new ArgumentException("Reservation has no Id.");
            if (string.IsNullOrWhiteSpace(reservation.Name))
                throw new ArgumentException("Passenger name cannot be empty.");

            _dao.Update(reservation);
        }

        public void Delete(int id)
        {
            if (id <= 0) throw new ArgumentException("Id must be positive.");

            // Free up seats before deleting
            var reservation = _dao.GetById(id);
            if (reservation != null)
            {
                var race = _raceDao.GetById(reservation.RaceId);
                if (race != null)
                {
                    foreach (var seat in reservation.Seats)
                        if (seat >= 0 && seat < race.AvailableSeats.Count)
                            race.AvailableSeats[seat] = true;

                    _raceDao.Update(race);
                }
            }

            _dao.Delete(id);
        }

        // Extra: all reservations for a specific race
        public List<Reservation> GetByRace(int raceId) =>
            _dao.GetAll().Where(r => r.RaceId == raceId).ToList();
    }
}