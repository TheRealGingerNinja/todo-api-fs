using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Todo.API.Data.Entities;
using Todo.API.Services;

namespace Todo.API.UnitTests.Mocks.Services
{
    public static class UserServiceMock
    {
        public static IUserService Setup(UserManager<User> manager, List<User> data)
        {
            var mock = new Mock<IUserService>();

            mock.Setup(m => m.GetUsers())
                .Returns(data.AsQueryable());

            mock.Setup(m => m.GetUserById(It.IsAny<long>()))
                .Returns((long id) => Task.FromResult(data.FirstOrDefault(e => e.Id == id)));

            mock.Setup(m => m.GetUserByEmailAsync(It.IsAny<string>()))
                .Returns((string email) => Task.FromResult(data.FirstOrDefault(e => string.Equals(e.Email, email, StringComparison.InvariantCultureIgnoreCase))));

            mock.Setup(m => m.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback(async (string email, string password, string role) =>
                {
                    var user = new User()
                    {
                        Email = email,
                        EmailConfirmed = true,
                        UserName = email,
                        SecurityStamp = Guid.NewGuid().ToString()
                    };

                    var createTask = await manager.CreateAsync(user, password);

                    if (!createTask.Succeeded)
                    {
                        var message = createTask.Errors.First().Description;

                        throw new Exception(message);
                    }

                    var entity = await manager.FindByEmailAsync(email);

                    data.Add(entity);

                    var addTask = await manager.AddToRoleAsync(user, role);

                    if (!addTask.Succeeded)
                    {
                        var message = createTask.Errors.First().Description;

                        throw new Exception(message);
                    }
                })
                .Returns((string email, string password, string role) => Task.FromResult(data.FirstOrDefault(e => string.Equals(e.Email, email, StringComparison.InvariantCultureIgnoreCase))));

            mock.Setup(m => m.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .Returns((User user, string password) => manager.CheckPasswordAsync(user, password));

            mock.Setup(m => m.CreateAuthorizationToken(It.IsAny<User>()))
                .Returns(async (User user) =>
                {
                    var roles = await manager.GetRolesAsync(user);

                    var claims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Email, user.Email),
                        new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };

                    claims.AddRange(roles.Select(r => new Claim("role", r)));

                    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("secret"));

                    var expiration = DateTime.Now.AddMinutes(15);

                    var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

                    return new JwtSecurityToken(claims: claims, expires: expiration, signingCredentials: signingCredentials);
                });

            return mock.Object;
        }
    }
}
