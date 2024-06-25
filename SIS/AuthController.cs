using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SIS.Models;
using Microsoft.Extensions.Logging;
using SIS.Data;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace SIS
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        public IConfiguration Configuration { get; }
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AuthController(SignInManager<User> signInManager, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            this._signInManager = signInManager;
            this._userManager = userManager;
            _roleManager = roleManager;
            this.Configuration = configuration;
        }
        [HttpGet]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult Get()
        {
            return Ok(new { Token = "" });
        }
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:SecretKey"]));

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Issuer"]));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: Configuration["Jwt:Issuer"],
                audience: Configuration["Jwt:Audience"],
                claims: authClaims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                
            );

            return token;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LoginModel usr)
        {
            if (usr is null)
            {
                return BadRequest("Invalid client request");
            }
            var result = await _signInManager.PasswordSignInAsync(usr.Email, usr.Password, false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(usr.Email);
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                var token = GetToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
                //var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Issuer"]));
                //var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                //var tokeOptions = new JwtSecurityToken(
                //    issuer: Configuration["Jwt:Issuer"],
                //    audience: Configuration["Jwt:Audience"],
                //    //claims: new List<Claim>(),
                //    expires: DateTime.Now.AddMinutes(5),
                //    signingCredentials: signinCredentials
                //);
                //var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                //return Ok(new { Token = tokenString });
                ////return Ok(new { Token = "" });
            }
            else
            {
                return Unauthorized();
            }
        }
        //public async Task<IActionResult> Post([FromBody] LoginModel user)
        //{
        //    if (user is null)
        //    {
        //        return BadRequest("Invalid client request");
        //    }
        //    var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, false, lockoutOnFailure: false);
        //    return Ok(new { Token = "" });
        //    //if (user.UserName == "johndoe" && user.Password == "def@123")
        //    //{
        //    //    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
        //    //    var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        //    //    var tokeOptions = new JwtSecurityToken(
        //    //        issuer: "https://localhost:5001",
        //    //        audience: "https://localhost:5001",
        //    //        claims: new List<Claim>(),
        //    //        expires: DateTime.Now.AddMinutes(5),
        //    //        signingCredentials: signinCredentials
        //    //    );
        //    //    var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        //    //    return Ok(new AuthenticatedResponse { Token = tokenString });
        //    //}
        //    //return Unauthorized();
        //}
    }
}
