using AuthMs.DataB;
using AuthMs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace AuthMs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static Credential credential = new Credential();
        
        private readonly IConfiguration _configuration;

        private readonly DataBaseContext _dataBaseContext;
        

        public AuthController(IConfiguration configuration, DataBaseContext dataBaseContext) 
        {
            _configuration = configuration;
            _dataBaseContext = dataBaseContext; 

        }

        [HttpPost("register")]
        public async Task<ActionResult<Credential>> Register(CredentialDto request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            credential.UserId = request.UserId;
            credential.PasswordHash = passwordHash;
            credential.PasswordSalt = passwordSalt;

            //_dataBaseContext.Credentials.Add(credential);
            //await _dataBaseContext.SaveChanges();

            //throw new Exception(ConfigurationManager.AppSettings[paramName["mcn"].ConnectionString);

            return Ok(credential);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(CredentialDto request)
        {
            if (credential.UserId != request.UserId)
            {
                return BadRequest("Usuario no encontrado");
            }

            if (!VerificarPasswordHash(request.Password, credential.PasswordHash, credential.PasswordSalt))
            {
                return BadRequest("Contraseña incorrecta");
            }

            string token = CreateToken(credential);

            return Ok("Bienvenido"); 
        }

        private string CreateToken(Credential credential)
        {
            var StringUserId = credential.UserId.ToString();

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, StringUserId),
                new Claim(ClaimTypes.Role, "Pilot")
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                                        _configuration.GetSection("AppSettings:Token").Value));

            var creden = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creden
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        private bool VerificarPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);

        }
    }
}
