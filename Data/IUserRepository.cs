using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAuth.Models;

namespace WebAuth.Data
{
    public interface IUserRepository
    {
        Task<User> Create(User user);
        Task<User> GetByEmail(string email);
        Task<User> GetById(int id);
    }
}
