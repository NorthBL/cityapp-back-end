using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAuth.Dtos;
using WebAuth.Models;

namespace WebAuth.interfaces
{
    public interface IAuthService
    {
        Task<User> CreateUser(RegisterDto dto);

        Task<User> LoginUser(LoginDto dto);

        Task<User> UserExist(string jwt);
    }
}
