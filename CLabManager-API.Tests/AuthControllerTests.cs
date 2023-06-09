using CLabManager_API.Controllers;
using CLabManager_API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ModelsLibrary.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLabManager_API.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
        private readonly Mock<JwtService> _mockJwtService;
        private readonly AuthController _authController;
        private readonly Mock<IConfiguration> _mockConfiguration;
        public AuthControllerTests()
        {
            _mockUserManager = new Mock<UserManager<IdentityUser>>(Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
            _mockConfiguration = new Mock<IConfiguration>();
            _mockJwtService = new Mock<JwtService>(_mockConfiguration.Object, _mockUserManager.Object);
            _authController = new AuthController(_mockUserManager.Object, _mockJwtService.Object);
        }
        [Fact]
        public async void SignIn_GivenUserData_ReturnLoginResponse()
        {
            _mockUserManager.Setup(r => r.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(CreateUser());
            _mockUserManager.Setup(r => r.CheckPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(true);
            _mockJwtService.Setup(r => r.CreateToken(CreateUser()));
            var userData = new SignInUser
            {
                Email = "something@gmail.com",
                Password = "password"
            };
            var result = await _authController.SignIn(userData);
            var loginResponse = result.Value;
            Assert.IsType<LoginResponse>(loginResponse);
        }
        [Fact]
        public async void SignIn_WithInvalidModelState_ReturnsBadRequestObjectResult()
        {
            var userData = new SignInUser
            {
                Email = "somethinggmail.com",
                Password = "password"
            };
            _authController.ModelState.AddModelError("email", "invalid");
            var result = await _authController.SignIn(userData);
            var BadRequestObjectResult = result.Result;
            Assert.IsAssignableFrom<BadRequestObjectResult>(BadRequestObjectResult);

        }
        [Fact]
        public async void SignIn_EmailNotFound_ReturnNotFoundResult()
        {
            _mockUserManager.Setup(r => r.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(() => null);
            _mockUserManager.Setup(r => r.CheckPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(true);
            _mockJwtService.Setup(r => r.CreateToken(CreateUser()));
            var userData = new SignInUser
            {
                Email = "something@gmail.com",
                Password = "password"
            };
            var result = await _authController.SignIn(userData);
            var NotFound = result.Result;
            Assert.IsType<NotFoundResult>(NotFound);
        }
        [Fact]
        public async void SignIn_IncorrectPassword_ReturnNotFoundResult()
        {
            _mockUserManager.Setup(r => r.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(CreateUser());
            _mockUserManager.Setup(r => r.CheckPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(false);
            _mockJwtService.Setup(r => r.CreateToken(CreateUser()));
            var userData = new SignInUser
            {
                Email = "something@gmail.com",
                Password = "password"
            };
            var result = await _authController.SignIn(userData);
            var NotFound = result.Result;
            Assert.IsType<NotFoundResult>(NotFound);
        }
        [Fact]
        public async void SignUp_GiverUserData_ReturnsAuthenticationResponse()
        {
            _mockUserManager.Setup(r => r.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).Returns(CreateIdentityResult());
            _mockUserManager.Setup(r => r.CreateAsync(It.IsAny<IdentityUser>(),It.IsAny<string>())).Returns(CreateIdentityResult());
            _mockUserManager.Setup(r => r.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(CreateUser());
            _mockJwtService.Setup(r => r.CreateToken(It.IsAny<IdentityUser>())).Returns(CreateAuthenticationResponse());
            var userData = new SignUpUser 
            {
                Email = "something@gmail.com",
                Password = "password",
                UserName = "someone" 
            };
            var result = await _authController.SignUp(userData);
            var authenticationResponse = result.Value;
            Assert.IsType<AuthenticationResponse>(authenticationResponse);

        }
        private AuthenticationResponse CreateAuthenticationResponse()
        {
            return new AuthenticationResponse
            {
                Token = "dummytoken",
                Expiration = DateTime.UtcNow
            };
        }
        private Task<IdentityResult> CreateIdentityResult()
        {
            return Task.FromResult(IdentityResult.Success);
        }
        private IdentityUser CreateUser()
        {
            return new IdentityUser
            {
                Id = "asdfs",
                Email = "yahya.zf2@gmail.com",
                UserName = "Yahya"
            };
        }
    }
}
