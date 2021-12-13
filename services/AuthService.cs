using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAuth.Data;
using WebAuth.Dtos;
using WebAuth.Helpers;
using WebAuth.interfaces;
using WebAuth.Models;

namespace WebAuth.services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repository;
        private readonly JwtService _jwtService;

        public AuthService(IUserRepository repository, JwtService jwtService)
        {
            _repository = repository;
            _jwtService = jwtService;
        }

        public async Task<User> CreateUser(RegisterDto dto)
        {
            var user = new User()
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };
            try
            {
                await _repository.Create(user);
            } catch (Exception e)
            {
                return null;
            }

            return user;
        }

        public async Task<User> LoginUser(LoginDto dto)
        {
            var user = await _repository.GetByEmail(dto.Email);

            return user;
        }

        public async Task<User> UserExist(string jwt)
        {
                var token = _jwtService.Verify(jwt);

                int userId = int.Parse(token.Issuer);

                var user = await _repository.GetById(userId);

                return user;
        }
    }
}
