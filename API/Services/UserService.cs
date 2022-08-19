using API.Dtos;
using API.Helpers;
using API.Services.Interfaces;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace API.Services
{
    public class UserService : IUserService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly JWTHelper _jwtHelper;

        public UserService(IUnitOfWork unitOfWork, IPasswordHasher<User> passwordHasher, IOptions<JWTHelper> jwtHelper)
        {
            this._unitOfWork = unitOfWork;
            this._passwordHasher = passwordHasher;
            this._jwtHelper = jwtHelper.Value;
        }

        public async Task<string> RegisterAsync(RegisterDto model)
        {
            var newUser = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Username = model.Username,
                Email = model.Email
            };

            newUser.Password = this._passwordHasher.HashPassword(newUser, model.Password);

            var userExists = this._unitOfWork.Users
                .Find(u => u.Username.ToLower() == model.Username.ToLower())
                .FirstOrDefault();
            if (userExists == null)
            {
                var defaultRole = this._unitOfWork.Roles
                    .Find(r => r.Name == AuthorizationHelper.default_user_role.ToString())
                    .First();

                try
                {
                    newUser.Roles.Add(defaultRole);
                    this._unitOfWork.Users.Add(newUser);
                    await this._unitOfWork.SaveAsync();

                    return $"The user '{model.Username}' has been successfully registered.";
                }
                catch (Exception ex)
                {
                    return $"Error: {ex.Message}";
                }
            }

            return $"Username '{model.Username}' already exists.";
        }

        public async Task<UserDataDto> GetTokenAsync(LoginDto model)
        {
            UserDataDto userData = new UserDataDto();

            var user = await this._unitOfWork.Users.GetUserByUsernameAsync(model.Username);
            if (user == null)
            {
                userData.IsAuthenticated = false;
                userData.Message = $"Username '{model.Username}' is not valid.";
                return userData;
            }

            var result = this._passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);
            if (result == PasswordVerificationResult.Success)
            {
                userData.IsAuthenticated = true;
                userData.Username = user.Username;
                userData.Email = user.Email;
                userData.Roles = user.Roles.Select(r => r.Name).ToList();
                JwtSecurityToken jwtSecurityToken = this.CreateJwtToken(user);
                userData.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

                if (user.RefreshTokens.Any(t => t.IsActive))
                {
                    var activeRefreshToken = user.RefreshTokens.Where(t => t.IsActive).FirstOrDefault();

                    userData.RefreshToken = activeRefreshToken.Token;
                    userData.RefreshTokenExpiration = activeRefreshToken.ExpirationDate;
                }
                else
                {
                    var refreshToken = this.CreateRefreshToken();

                    userData.RefreshToken = refreshToken.Token;
                    userData.RefreshTokenExpiration = refreshToken.ExpirationDate;

                    user.RefreshTokens.Add(refreshToken);
                    this._unitOfWork.Users.Update(user);
                    await this._unitOfWork.SaveAsync();
                }

                return userData;
            }

            userData.IsAuthenticated = false;
            userData.Message = $"Invalid login credentials for user '{model.Username}'";
            return userData;
        }

        private JwtSecurityToken CreateJwtToken(User user)
        {
            var roleClaims = new List<Claim>();
            foreach (var role in user.Roles)
            {
                roleClaims.Add(new Claim("roles", role.Name));
            }

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Username),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim("uid",user.Id.ToString())
            }.Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._jwtHelper.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                    issuer: this._jwtHelper.Issuer,
                    audience: this._jwtHelper.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(this._jwtHelper.DurationMinutes),
                    signingCredentials: signingCredentials
                );

            return jwtSecurityToken;
        }

        public async Task<UserDataDto> RefreshTokenAsync(string refreshToken)
        {
            var userData = new UserDataDto();

            var user = await this._unitOfWork.Users.GetUserByRefreshTokenAsync(refreshToken);
            if (user == null)
            {
                userData.IsAuthenticated = false;
                userData.Message = $"The token does not belong to any user.";
                return userData;
            }

            var refreshTokenEntity = user.RefreshTokens.Single(rt => rt.Token == refreshToken);
            if (!refreshTokenEntity.IsActive)
            {
                userData.IsAuthenticated = false;
                userData.Message = $"The token is not active";
                return userData;
            }

            refreshTokenEntity.RevokeDate = DateTime.UtcNow;

            var newRefreshToken = this.CreateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            this._unitOfWork.Users.Update(user);
            await this._unitOfWork.SaveAsync();

            userData.IsAuthenticated = true;
            JwtSecurityToken jwtSecurityToken = this.CreateJwtToken(user);
            userData.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            userData.Email = user.Email;
            userData.Username = user.Username;
            userData.Roles = user.Roles.Select(r => r.Name).ToList();
            userData.RefreshToken = newRefreshToken.Token;
            userData.RefreshTokenExpiration = newRefreshToken.ExpirationDate;

            return userData;
        }

        private RefreshToken CreateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomNumber);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomNumber),
                    ExpirationDate = DateTime.UtcNow.AddDays(10),
                    CreationDate = DateTime.UtcNow
                };
            }
        }

        public async Task<string> AddRoleAsync(AddRoleDto model)
        {
            var user = await this._unitOfWork.Users.GetUserByUsernameAsync(model.Username);
            if (user == null)
            {
                return $"There is no user for '{model.Username}'.";
            }

            var result = this._passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);
            if (result == PasswordVerificationResult.Success)
            {
                var role = this._unitOfWork.Roles.Find(r => r.Name.ToLower() == model.RoleName.ToLower()).FirstOrDefault();
                if (role != null)
                {
                    var userRoleExists = user.Roles.Any(r => r.Id == role.Id);
                    if (!userRoleExists)
                    {
                        user.Roles.Add(role);
                        this._unitOfWork.Users.Update(user);
                        await this._unitOfWork.SaveAsync();
                    }

                    return $"Role '{model.RoleName}' successfully added to user '{model.Username}'.";
                }
                return $"Role '{model.RoleName}' does not exist.";
            }
            return $"Invalid credentials for user '{model.Username}'.";
        }

    }
}

