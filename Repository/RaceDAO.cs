using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using MPP_Client.Models;

namespace MPP_Client.Repository
{
    public class RaceDAO : IRepository<Race, int>
    {
        public Race GetById(int id)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT id, destination, date, time, seats
                FROM race WHERE id = $id";
            cmd.Parameters.AddWithValue("$id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
                return MapRow(reader);

            return null;
        }

        public List<Race> GetAll()
        {
            var list = new List<Race>();

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id, destination, date, time, seats FROM race";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                list.Add(MapRow(reader));

            return list;
        }

        public void Add(Race race)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO race (destination, date, time, seats)
                VALUES ($destination, $date, $time, $seats)";

            cmd.Parameters.AddWithValue("$destination", race.Destination);
            cmd.Parameters.AddWithValue("$date",        race.Date);
            cmd.Parameters.AddWithValue("$time",        race.Time);
            cmd.Parameters.AddWithValue("$seats",       SerializeSeats(race.AvailableSeats));
            cmd.ExecuteNonQuery();

            cmd.CommandText = "SELECT last_insert_rowid()";
            race.Id = Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(Race race)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE race
                SET destination = $destination,
                    date        = $date,
                    time        = $time,
                    seats       = $seats
                WHERE id = $id";

            cmd.Parameters.AddWithValue("$destination", race.Destination);
            cmd.Parameters.AddWithValue("$date",        race.Date);
            cmd.Parameters.AddWithValue("$time",        race.Time);
            cmd.Parameters.AddWithValue("$seats",       SerializeSeats(race.AvailableSeats));
            cmd.Parameters.AddWithValue("$id",          race.Id);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM race WHERE id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }

        private static Race MapRow(SqliteDataReader r) => new Race(
            id:             r.GetInt32(0),
            destination:    r.GetString(1),
            date:           r.GetString(2),
            time:           r.GetString(3),
            availableSeats: DeserializeSeats(r.GetString(4))
        );

        // Matches Jackson's format exactly: [true,false,true]
        private static string SerializeSeats(List<bool> seats) =>
            JsonSerializer.Serialize(seats);

        private static List<bool> DeserializeSeats(string raw) =>
            JsonSerializer.Deserialize<List<bool>>(raw);
    }
}