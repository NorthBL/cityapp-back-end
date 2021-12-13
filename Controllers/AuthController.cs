using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebAuth.Data;
using WebAuth.Dtos;
using WebAuth.Helpers;
using WebAuth.interfaces;
using WebAuth.Models;
using WebAuth.services;

namespace WebAuth.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly JwtService _jwtService;
        private readonly IAuthService _authService;

        public AuthController(IUserRepository repository, JwtService jwtService, IAuthService authService)
        {
            _repository = repository;
            _jwtService = jwtService;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterDto dto)
        {
            var user = await _authService.CreateUser(dto);

            if (user == null)
            {
                return BadRequest(new { message = "User with this email already exist" });
            }

            return Created("success", user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginDto dto)
        {
            var user = await _authService.LoginUser(dto);
            
            if (user == null)
            {
                return BadRequest(new { message = "Ivalid Credentials" });
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {
                return BadRequest(new { message = "Ivalid Credentials" });
            }

            var jwt = _jwtService.Generate(user.Id);
            // http only cookie can not be accesed by the cliend(can only send this cookie) and can be getted in the backend and can modify and access it
            Response.Cookies.Append("jwt", jwt, new CookieOptions()); 

            return Ok(new { 
            message = "success"
            });
        }

        [HttpGet("user")]
        public async Task<IActionResult> User()
        {
            var jwt = Request.Cookies["jwt"];
            //Token can be invalid 
            try
            {
                var user = await _authService.UserExist(jwt);
                return Ok(user);

            } catch(Exception e)
            {
                return Unauthorized();
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return Ok(new
            {
                message = "success"
            });
        }

        [HttpGet("weather")]
        public async Task<IActionResult> GetWeatherForCity([FromQuery] string cityName)
        {
            var jwt = Request.Cookies["jwt"];
            //Token can be invalid 
            try
            {
                var user = _authService.UserExist(jwt);
                var client = new HttpClient();
                client.BaseAddress = new Uri("https://api.openweathermap.org/");

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                
                HttpResponseMessage Res = await client.GetAsync("data/2.5/weather?q=" + cityName + "&appid=6cae4c1b301dd1d3f0b921ff5d9849ae");

                  
                if (Res.IsSuccessStatusCode)
                {

                    var ObjResponse = Res.Content.ReadAsStringAsync().Result;
                    return Ok(ObjResponse);

                }
                return NotFound(new { message = "City not found" });
            }
            catch (Exception e)
            {
                return Unauthorized();
            }
            
        }
    }
}
