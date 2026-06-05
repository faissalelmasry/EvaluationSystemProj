using AutoMapper;
using EvaluationSystem.Application.DTOs.Auth;
using EvaluationSystem.Application.Exceptions;
using EvaluationSystem.Application.Helpers;
using EvaluationSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace EvaluationSystem.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger _logger;
        private readonly RoleManager<Role> _roleManager;
        private SignInManager<User> _signInManager;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        private readonly JwtHelper _jwtHelper;
        private readonly IMapper _mapper;
        public AuthService(
             UserManager<User> userManager,
             RoleManager<Role> roleManager,
             SignInManager<User> signInManager,
             IConfiguration config,
             ApplicationDbContext context,
                JwtHelper jwtHelper,
                IMapper mapper
             )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _config = config;
            _jwtHelper = jwtHelper;
            _context = context;
            _mapper = mapper;
        }
        public async Task RegisterAsync(RegisterDTO dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                throw new BadRequestException("User with this email already exists.");
            }
          var user = _mapper.Map<User>(dto);
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException(errors);
            }
            // Lma n-manage roles mn el Admin Page, nb2a nghyar da
            if (!await _roleManager.RoleExistsAsync("Evaluatee"))
                await _roleManager.CreateAsync(new Role { Name = "Evaluatee" });

            await _userManager.AddToRoleAsync(user, "Evaluatee");
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }
            var roles = await _userManager.GetRolesAsync(user);
            var (jwtToken, jwtExpiry) = _jwtHelper.GenerateJwtToken(user, roles);
            var refreshToken = await CreateAndSaveRefreshTokenAsync(user);
            return new AuthResponseDto
            {
                Token = jwtToken,
                TokenExpiresAt = jwtExpiry,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiresAt = refreshToken.ExpiresOn
            };
        }
      
        private async Task<RefreshToken> CreateAndSaveRefreshTokenAsync(User user)
        {
            var refreshToken = new RefreshToken
            {
                Token = _jwtHelper.GenerateSecureToken(),
                UserId = user.Id,
                CreatedOn = DateTime.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddDays(7)
            };
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
            return refreshToken;
        }
        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var token = await _context.RefreshTokens
                .Include(t => t.User)
                .SingleOrDefaultAsync(t => t.Token == refreshToken);

            if (token == null)
                throw new UnauthorizedException("Invalid refresh token");

            if (!token.IsActive)
            {
                var reason = token.IsExpired ? "Refresh token has expired" : "Refresh token has been revoked";
                throw new UnauthorizedException(reason);
            }

            token.RevokedOn = DateTime.UtcNow;

            var user = token.User;
            var roles = await _userManager.GetRolesAsync(user);

            var (jwtToken, jwtExpiry) = _jwtHelper.GenerateJwtToken(user, roles);
            var newRefreshToken = await CreateAndSaveRefreshTokenAsync(user);

            return new AuthResponseDto
            {
                Token = jwtToken,
                TokenExpiresAt = jwtExpiry,
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiresAt = newRefreshToken.ExpiresOn
            };
        }

        public async Task RevokeTokenAsync(string refreshToken)
        {
            var token = await _context.RefreshTokens
                .SingleOrDefaultAsync(t => t.Token == refreshToken);

            if (token == null)
                throw new BadRequestException("Invalid refresh token");

            if (!token.IsActive)
                throw new BadRequestException("Token is already inactive");

            token.RevokedOn = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
