using Apresentacao.Models;
using System.Collections.Generic;

namespace Apresentacao.Services
{
    public interface IUserService
    {
        User GetById(int id);
        User GetByLogin(string login, string password);
        IList<User> GetAll();
    }
}