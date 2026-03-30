using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using MPP_Client.Models;

namespace MPP_Client.Repository
{
    public class ReservationDAO : IRepository<Reservation, int>
    {
        public Reservation GetById(int id)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT id, id_user, id_race, name, seats
                FROM reservation WHERE id = $id";
            cmd.Parameters.AddWithValue("$id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
                return MapRow(reader);

            return null;
        }

        public List<Reservation> GetAll()
        {
            var list = new List<Reservation>();

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id, id_user, id_race, name, seats FROM reservation";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                list.Add(MapRow(reader));

            return list;
        }

        public void Add(Reservation reservation)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO reservation (id_user, id_race, name, seats)
                VALUES ($userId, $raceId, $name, $seats)";

            cmd.Parameters.AddWithValue("$userId", reservation.UserId);
            cmd.Parameters.AddWithValue("$raceId", reservation.RaceId);
            cmd.Parameters.AddWithValue("$name",   reservation.Name);
            cmd.Parameters.AddWithValue("$seats",  SerializeSeats(reservation.Seats));
            cmd.ExecuteNonQuery();

            cmd.CommandText = "SELECT last_insert_rowid()";
            reservation.Id = Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(Reservation reservation)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE reservation
                SET id_user = $userId,
                    id_race = $raceId,
                    name    = $name,
                    seats   = $seats
                WHERE id = $id";

            cmd.Parameters.AddWithValue("$userId", reservation.UserId);
            cmd.Parameters.AddWithValue("$raceId", reservation.RaceId);
            cmd.Parameters.AddWithValue("$name",   reservation.Name);
            cmd.Parameters.AddWithValue("$seats",  SerializeSeats(reservation.Seats));
            cmd.Parameters.AddWithValue("$id",     reservation.Id);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM reservation WHERE id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }

        private static Reservation MapRow(SqliteDataReader r) => new Reservation(
            id:     r.GetInt32(0),
            userId: r.GetInt32(1),
            raceId: r.GetInt32(2),
            name:   r.GetString(3),
            seats:  DeserializeSeats(r.GetString(4))
        );

        // Matches Jackson's format exactly: [1,4,7]
        private static string SerializeSeats(List<int> seats) =>
            JsonSerializer.Serialize(seats);

        private static List<int> DeserializeSeats(string raw) =>
            JsonSerializer.Deserialize<List<int>>(raw);
    }
}