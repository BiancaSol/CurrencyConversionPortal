namespace CurrencyConversionPortal.Api.Tests.Controllers
{
    using CurrencyConversionPortal.Api.Controllers;
    using CurrencyConversionPortal.Api.DTOs;
    using CurrencyConversionPortal.Core.Exceptions;
    using CurrencyConversionPortal.Core.Services;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using System.Security.Claims;

    public class AuthControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IAuthenticationService> _mockAuthenticationService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockAuthenticationService = new Mock<IAuthenticationService>();
            _controller = new AuthController(_mockUserService.Object);

            var httpContext = new DefaultHttpContext();
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(sp => sp.GetService(typeof(IAuthenticationService)))
                          .Returns(_mockAuthenticationService.Object);
            httpContext.RequestServices = serviceProvider.Object;

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }


        [Fact]
        public void Register_WithValidData_ReturnsCreatedResult()
        {
            var registerDto = new RegisterUserDto
            {
                Username = "testuser",
                Password = "password123"
            };
            _mockUserService.Setup(x => x.Register(registerDto.Username, registerDto.Password))
                           .Returns(true);


            var result = _controller.Register(registerDto);


            var createdResult = Assert.IsType<CreatedResult>(result);
            Assert.NotNull(createdResult.Value);
            var username = createdResult.Value.GetType().GetProperty("username")?.GetValue(createdResult.Value);
            Assert.Equal("testuser", username);
        }

        [Fact]
        public void Register_WithExistingUsername_ReturnsConflictResult()
        {
            var registerDto = new RegisterUserDto
            {
                Username = "existinguser",
                Password = "password123"
            };
            _mockUserService.Setup(x => x.Register(registerDto.Username, registerDto.Password))
                           .Returns(false);

            var result = _controller.Register(registerDto);

            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("Username already exists.", conflictResult.Value);
        }

        [Fact]
        public void Register_WhenUserServiceThrowsArgumentException_PropagatesException()
        {
            var registerDto = new RegisterUserDto
            {
                Username = "testuser",
                Password = "password123"
            };
            _mockUserService.Setup(x => x.Register(registerDto.Username, registerDto.Password))
                           .Throws(new ArgumentException("Username contains invalid characters", "userName"));


            var exception = Assert.Throws<ArgumentException>(() => _controller.Register(registerDto));
            Assert.Equal("userName", exception.ParamName);
            Assert.Contains("Username contains invalid characters", exception.Message);
        }

        [Fact]
        public void Register_WhenUserServiceThrowsValidationException_PropagatesException()
        {
            var registerDto = new RegisterUserDto
            {
                Username = "testuser",
                Password = "password123"
            };
            _mockUserService.Setup(x => x.Register(registerDto.Username, registerDto.Password))
                           .Throws(new ValidationException("Password does not meet security requirements"));


            var exception = Assert.Throws<ValidationException>(() => _controller.Register(registerDto));
            Assert.Equal("Password does not meet security requirements", exception.Message);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkResultAndSignsIn()
        {
            var loginDto = new LoginUserDto
            {
                Username = "testuser",
                Password = "password123"
            };
            _mockUserService.Setup(x => x.ValidateCredentials(loginDto.Username, loginDto.Password))
                           .Returns(true);

            _mockAuthenticationService.Setup(x => x.SignInAsync(
                It.IsAny<HttpContext>(),
                CookieAuthenticationDefaults.AuthenticationScheme,
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);


            var result = await _controller.Login(loginDto);


            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            var message = okResult.Value.GetType().GetProperty("message")?.GetValue(okResult.Value);
            Assert.Equal("Login successful.", message);

            _mockAuthenticationService.Verify(x => x.SignInAsync(
                It.IsAny<HttpContext>(),
                CookieAuthenticationDefaults.AuthenticationScheme,
                It.Is<ClaimsPrincipal>(p => p.Identity!.Name == loginDto.Username),
                It.IsAny<AuthenticationProperties>()), Times.Once);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorizedResult()
        {
            var loginDto = new LoginUserDto
            {
                Username = "testuser",
                Password = "wrongpassword"
            };
            _mockUserService.Setup(x => x.ValidateCredentials(loginDto.Username, loginDto.Password))
                           .Returns(false);


            var result = await _controller.Login(loginDto);


            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid username or password.", unauthorizedResult.Value);

            _mockAuthenticationService.Verify(x => x.SignInAsync(
                It.IsAny<HttpContext>(),
                It.IsAny<string>(),
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<AuthenticationProperties>()), Times.Never);
        }

        [Fact]
        public async Task Login_WithValidCredentials_CreatesCorrectClaims()
        {
            var loginDto = new LoginUserDto
            {
                Username = "testuser",
                Password = "password123"
            };
            _mockUserService.Setup(x => x.ValidateCredentials(loginDto.Username, loginDto.Password))
                           .Returns(true);

            ClaimsPrincipal? capturedPrincipal = null;
            _mockAuthenticationService.Setup(x => x.SignInAsync(
                It.IsAny<HttpContext>(),
                CookieAuthenticationDefaults.AuthenticationScheme,
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<AuthenticationProperties>()))
                .Callback<HttpContext, string, ClaimsPrincipal, AuthenticationProperties>((context, scheme, principal, properties) =>
                {
                    capturedPrincipal = principal;
                })
                .Returns(Task.CompletedTask);


            await _controller.Login(loginDto);


            Assert.NotNull(capturedPrincipal);
            Assert.Equal(CookieAuthenticationDefaults.AuthenticationScheme, capturedPrincipal.Identity!.AuthenticationType);
            Assert.True(capturedPrincipal.Identity.IsAuthenticated);
            Assert.Equal("testuser", capturedPrincipal.Identity.Name);

            var nameClaim = capturedPrincipal.FindFirst(ClaimTypes.Name);
            Assert.NotNull(nameClaim);
            Assert.Equal("testuser", nameClaim.Value);
        }

        [Fact]
        public async Task Logout_ReturnsOkResultAndSignsOut()
        {
            _mockAuthenticationService.Setup(x => x.SignOutAsync(
                It.IsAny<HttpContext>(),
                CookieAuthenticationDefaults.AuthenticationScheme,
                It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);


            var result = await _controller.Logout();


            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            var message = okResult.Value.GetType().GetProperty("message")?.GetValue(okResult.Value);
            Assert.Equal("Logged out successfully.", message);

            _mockAuthenticationService.Verify(x => x.SignOutAsync(
                It.IsAny<HttpContext>(),
                CookieAuthenticationDefaults.AuthenticationScheme,
                It.IsAny<AuthenticationProperties>()), Times.Once);
        }
    }
}