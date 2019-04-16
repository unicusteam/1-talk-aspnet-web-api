using Apresentacao.Models;
using System.Collections.Generic;
using System.Linq;

namespace Apresentacao.Services
{
    public class UserService : IUserService
    {
        public User GetById(int id)
        {
            return GetAll().FirstOrDefault(c => c.Id == id);
        }

        public User GetByLogin(string login, string password)
        {
            return GetAll().FirstOrDefault(c => c.Login == login && c.Password == password);
        }

        public IList<User> GetAll()
        {
            return new List<User>
            {
                new User
                {
                    Id = 1,
                    Name = "Master",
                    Login = "master",
                    Password = "123"
                },
                new User
                {
                    Id = 2,
                    Name = "Jonathan",
                    Login = "jonathan",
                    Password = "123"
                }
            };
        }
    }
}
