using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using GraphQL.PureCodeFirst.Auth.Data;
using GraphQL.PureCodeFirst.Auth.Data.Entities;
using GraphQL.PureCodeFirst.Auth.Shared;
using GraphQL.PureCodeFirst.InputTypes;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GraphQL.PureCodeFirst.Auth.Logics
{
    public class AuthLogic : IAuthLogic
    {
        private readonly AuthContext _authContext;
        private readonly TokenSettings _tokenSettings;
        public AuthLogic(AuthContext authContext,
           IOptions<TokenSettings> tokenSettings)
        {
            _authContext = authContext;
            _tokenSettings = tokenSettings.Value;
        }
        private string ResigstrationValidations(RegisterInputType registerInput)
        {
            if (string.IsNullOrEmpty(registerInput.EmailAddress))
            {
                return "Eamil can't be empty";
            }

            if (string.IsNullOrEmpty(registerInput.Password)
                || string.IsNullOrEmpty(registerInput.ConfirmPassword))
            {
                return "Password Or ConfirmPasswor Can't be empty";
            }

            if (registerInput.Password != registerInput.ConfirmPassword)
            {
                return "Invalid confirm password";
            }

            string emailRules = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
            if (!Regex.IsMatch(registerInput.EmailAddress, emailRules))
            {
                return "Not a valid email";
            }

            // atleast one lower case letter
            // atleast one upper case letter
            // atleast one special character
            // atleast one number
            // atleast 8 character length
            string passwordRules = @"^.*(?=.{8,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!*@#$%^&+=]).*$";
            if (!Regex.IsMatch(registerInput.Password, passwordRules))
            {
                return "Not a valid password";
            }

            return string.Empty;
        }

        private string PasswordHash(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 1000);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            return Convert.ToBase64String(hashBytes);
        }
        public string Register(RegisterInputType registerInput)
        {
            string errorMessage = ResigstrationValidations(registerInput);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return errorMessage;
            }

            var newUser = new User
            {
                EmailAddress = registerInput.EmailAddress,
                FirstName = registerInput.FirstName,
                LastName = registerInput.LastName,
                Password = PasswordHash(registerInput.ConfirmPassword)
            };

            _authContext.User.Add(newUser);
            _authContext.SaveChanges();

            // default role on registration
            var newUserRoles = new UserRoles
            {
                Name = "admin",
                UserId = newUser.UserId
            };

            _authContext.UserRoles.Add(newUserRoles);
            _authContext.SaveChanges();

            return "Registration success";
        }

        private bool ValidatePasswordHash(string password, string dbPassword)
        {
            byte[] hashBytes = Convert.FromBase64String(dbPassword);

            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 1000);
            byte[] hash = pbkdf2.GetBytes(20);

            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                {
                    return false;
                }
            }

            return true;
        }

        private string GetJWTAuthKey(User user, List<UserRoles> roles)
        {
            var securtityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.Key));

            var credentials = new SigningCredentials(securtityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>();

            claims.Add(new Claim("Email", user.EmailAddress));
            claims.Add(new Claim("LastName", user.LastName));
            if ((roles?.Count ?? 0) > 0)
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Name));
                }
            }

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _tokenSettings.Issuer,
                audience: _tokenSettings.Audience,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials,
                claims:claims
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }
        public string Login(LoginInputType loginInput)
        {
            if (string.IsNullOrEmpty(loginInput.Email)
            || string.IsNullOrEmpty(loginInput.Password))
            {
                return "Invalid Credentials";
            }

            var user = _authContext.User.Where(_ => _.EmailAddress == loginInput.Email).FirstOrDefault();
            if (user == null)
            {
                return "Invalid Credentials";
            }

            if (!ValidatePasswordHash(loginInput.Password, user.Password))
            {
                return "Invalid Credentials";
            }

            var roles = _authContext.UserRoles.Where(_ => _.UserId == user.UserId).ToList();
            
            return GetJWTAuthKey(user, roles);
        }
    }
}