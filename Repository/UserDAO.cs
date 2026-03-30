using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using MPP_Client.Models;

namespace MPP_Client.Repository
{
    public class UserDAO : IRepository<User, int>
    {
        public User GetById(int id)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id, name, email, password FROM user WHERE id = $id";
            cmd.Parameters.AddWithValue("$id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
                return MapRow(reader);

            return null;
        }

        public List<User> GetAll()
        {
            var list = new List<User>();

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id, name, email, password FROM user";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                list.Add(MapRow(reader));

            return list;
        }

        public void Add(User user)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO user (name, email, password)
                VALUES ($name, $email, $password)";

            cmd.Parameters.AddWithValue("$name",     user.Name);
            cmd.Parameters.AddWithValue("$email",    user.Email);
            cmd.Parameters.AddWithValue("$password", user.Password);
            cmd.ExecuteNonQuery();

            cmd.CommandText = "SELECT last_insert_rowid()";
            user.Id = Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(User user)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE user
                SET name     = $name,
                    email    = $email,
                    password = $password
                WHERE id = $id";

            cmd.Parameters.AddWithValue("$name",     user.Name);
            cmd.Parameters.AddWithValue("$email",    user.Email);
            cmd.Parameters.AddWithValue("$password", user.Password);
            cmd.Parameters.AddWithValue("$id",       user.Id);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM user WHERE id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }

        private static User MapRow(SqliteDataReader r) => new User(
            id:       r.GetInt32(0),
            name:     r.GetString(1),
            email:    r.GetString(2),
            password: r.GetString(3)
        );
    }
}