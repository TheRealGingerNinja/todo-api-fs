using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Todo.API.Data.Entities;

namespace Todo.API.UnitTests.Mocks.Services
{
    public static class UserManagerMock
    {
        private static readonly IUserStore<User> userStore = new Mock<IUserStore<User>>().Object;

        private static readonly IOptions<IdentityOptions> options = new Mock<IOptions<IdentityOptions>>().Object;

        private static readonly PasswordHasher<User> hasher = new PasswordHasher<User>();

        private static readonly ILookupNormalizer normalizer = new Mock<ILookupNormalizer>().Object;

        private static readonly IdentityErrorDescriber errorDescriber = new Mock<IdentityErrorDescriber>().Object;

        private static readonly IServiceProvider serviceProvider = new Mock<IServiceProvider>().Object;

        private static readonly ILogger<UserManager<User>> logger = new Mock<ILogger<UserManager<User>>>().Object;

        public static UserManager<User> Setup(List<User> data)
        {
            var userManager = new Mock<UserManager<User>>(userStore, options, hasher, null, null, normalizer, errorDescriber, serviceProvider, logger);

            userManager
              .Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
              .ReturnsAsync(IdentityResult.Success)
              .Callback<User, string>((user, b) => data.Add(user));

            userManager
                .Setup(um => um.Users)
                .Returns(data.AsQueryable());

            userManager
                .Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<User>());

            userManager
                .Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<User>());

            userManager
                .Setup(um => um.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<bool>());

            userManager
                .Setup(um => um.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(new[] { "User" });

            return userManager.Object;
        }
    }
}
