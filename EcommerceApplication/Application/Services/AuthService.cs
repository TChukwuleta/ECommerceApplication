using EcommerceApplication.Data;
using EcommerceApplication.Data.Configuration;
using EcommerceApplication.Domain.DTOs.Response;
using EcommerceApplication.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace EcommerceApplication.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        private readonly JwtConfig _jwtConfig;
        private readonly IConfiguration _config;

        public AuthService(UserManager<ApplicationUser> userManager, AppDbContext context, IOptionsMonitor<JwtConfig> jwtConfig, IConfiguration config)
        {
            _userManager = userManager;
            _context = context;
            _jwtConfig = jwtConfig.CurrentValue;
            _config = config;
        }
        public async Task<ResultResponse> CreateUserAsync(Customer user)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(user.Email);
                if (existingUser != null)
                {
                    return ResultResponse.Failure("Email Already exist");
                }

                var newUser = new ApplicationUser
                {
                    FirstName = user.FirstName,
                    Name = user.FirstName + " " + user.LastName,
                    UserName = user.FirstName,
                    LastName = user.LastName
                };

                if(user.Email != null)
                {
                    newUser.EmailConfirmed = true;
                    newUser.Email = user.Email;
                    newUser.NormalizedEmail = user.Email;
                }

                newUser.UserId = newUser.Id;

                var result = await _userManager.CreateAsync(newUser, user.Password);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(c => c.Description);
                    return ResultResponse.Failure(errors);
                }

                var customer = new Customer
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Password = user.Password,
                    Phone = user.Phone,
                    Address = user.Address,
                    State = Domain.Enums.UserState.New,
                    StateDesc = Domain.Enums.UserState.New.ToString(),
                    CreatedDate = DateTime.Now,
                    UserId = newUser.UserId
                };

                await _context.Customers.AddAsync(customer);
                await _context.SaveChangesAsync();

                var jwtToken = GenerateJwtToken(newUser.Id, newUser.Email);
                return ResultResponse.Success(jwtToken);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<ResultResponse> ChangeUserStatusAsync(Customer user) 
        {
            try
            {
                var appUser = await _userManager.Users.FirstOrDefaultAsync(v => v.UserId == user.UserId);
                if (appUser == null)
                {
                    return ResultResponse.Failure("Not a user");
                }
                var appCustomer = await _context.Customers.Where(c => c.UserId == user.UserId).FirstOrDefaultAsync();
                string message = "";
                if(appCustomer != null)
                {
                    switch (appCustomer.State)
                    {
                        case Domain.Enums.UserState.New:
                            appCustomer.State = Domain.Enums.UserState.New;
                            message = "User was created successfully";
                            break;
                        case Domain.Enums.UserState.Active:
                            appCustomer.State = Domain.Enums.UserState.Active;
                            message = "User activation was successful";
                            break;
                        case Domain.Enums.UserState.Blocked:
                            appCustomer.State = Domain.Enums.UserState.Blocked;
                            message = "User has been blocked succeessfully";
                            break;
                        case Domain.Enums.UserState.Banned:
                            appCustomer.State = Domain.Enums.UserState.Banned;
                            message = "User has been banned successfully";
                            break;
                        default:
                            break;
                    }
                    _context.Customers.Update(appCustomer);
                    await _context.SaveChangesAsync();
                }
                return ResultResponse.Success(appCustomer);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<ResultResponse> loginAsync(string email, string password)
        {
            try
            {
                var user = await _context.Customers.Where(c => c.Email == email).FirstOrDefaultAsync();
                if(user == null)
                {
                    return ResultResponse.Failure("Not a valiid user");
                }
                var appUser = await _userManager.FindByIdAsync(user.UserId);
                if (appUser == null)
                {
                    return ResultResponse.Failure("User does not exist");
                }
                var isValidUser = await _userManager.CheckPasswordAsync(appUser, password);
                if (!isValidUser)
                {
                    return ResultResponse.Failure("Invalid Login request");
                }
                var jwtToken = GenerateJwtToken(appUser.Id, user.Email);
                return ResultResponse.Success(jwtToken);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<ResultResponse> ChangePasswordAsync(string email, string oldPassword, string newPassword)
        {
            try
            {
                var appUser = await _userManager.FindByEmailAsync(email);
                if (appUser == null)
                {
                    return ResultResponse.Failure("User does not exist");
                }
                var checkPassword = await _userManager.CheckPasswordAsync(appUser, newPassword);
                if (checkPassword)
                {
                    return ResultResponse.Failure("Please use a new password");
                }
                var changePassword = await _userManager.ChangePasswordAsync(appUser, oldPassword, newPassword);
                if(!changePassword.Succeeded)
                {
                    return ResultResponse.Success("Password change was not successful");
                }
                return ResultResponse.Success("Password changed successfully");
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private string GenerateJwtToken(string Id, string email)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            /*var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);*/
            var key = Encoding.ASCII.GetBytes(_config["TokenConstants:key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", Id),
                    new Claim(JwtRegisteredClaimNames.Email, email),
                    new Claim(JwtRegisteredClaimNames.Sub, email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(6),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }


       /* public async Task<AuthToken> GenerateAccessToken(User user)
        {
            try
            {
                //TODO: Add role access level
                List<Claim> claims = new List<Claim>() {
                new Claim (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim (JwtRegisteredClaimNames.Email, user.Email),
                 new Claim ("userid", user.UserId),
                 new Claim ("bvn", string.IsNullOrWhiteSpace(user.BVN) ? "": user.BVN),
                 new Claim ("useraccesslevel", user.UserAccessLevel.ToString()),
                 new Claim ("roleaccesslevel",  user.UserAccessLevel.ToString())
                };
                JwtSecurityToken token = new TokenBuilder()
               .AddAudience(_configuration["Token:aud"])
               .AddIssuer(_configuration["Token:issuer"])
               .AddExpiry(Convert.ToInt32(_configuration["TokenConstants:ExpiryInMinutes"]))
               .AddKey(Encoding.UTF8.GetBytes(_configuration["TokenConstants:key"]))
               .AddClaims(claims)
               .Build();

                string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

                var newToken = new AuthToken()
                {
                    AccessToken = accessToken,
                    ExpiresIn = Convert.ToInt32(_configuration["TokenConstants:ExpiryInMinutes"]),
                };
                return await Task.FromResult(newToken);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }*/
    }
}
