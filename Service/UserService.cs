using System;
using System.Collections.Generic;
using MPP_Client.Models;
using MPP_Client.Repository;

namespace MPP_Client.Service
{
    public class UserService : IService<User, int>
    {
        private readonly UserDAO _dao;

        public UserService(UserDAO userDAO)
        {
            this._dao = userDAO;
        }

        public User GetById(int id)
        {
            if (id <= 0) throw new ArgumentException("Id must be positive.");
            return _dao.GetById(id);
        }

        public List<User> GetAll() => _dao.GetAll();

        public void Add(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Name))
                throw new ArgumentException("Name cannot be empty.");
            if (string.IsNullOrWhiteSpace(user.Email) || !user.Email.Contains('@'))
                throw new ArgumentException("Invalid email address.");
            if (string.IsNullOrWhiteSpace(user.Password) || user.Password.Length < 4)
                throw new ArgumentException("Password must be at least 4 characters.");

            _dao.Add(user);
        }

        public void Update(User user)
        {
            if (user.Id == null) throw new ArgumentException("User has no Id.");
            if (string.IsNullOrWhiteSpace(user.Name))
                throw new ArgumentException("Name cannot be empty.");
            if (string.IsNullOrWhiteSpace(user.Email) || !user.Email.Contains('@'))
                throw new ArgumentException("Invalid email address.");

            _dao.Update(user);
        }

        public void Delete(int id)
        {
            if (id <= 0) throw new ArgumentException("Id must be positive.");
            _dao.Delete(id);
        }

        // Extra: used by login form to validate credentials
        public User FindByEmailAndPassword(string email, string password)
        {
            return _dao.GetAll().Find(u =>
                u.Email    == email &&
                u.Password == password);
        }
    }
}