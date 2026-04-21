using Microsoft.EntityFrameworkCore;
using MockHackothonBackend.Data;
using MockHackothonBackend.DTOs.Auth;
using MockHackothonBackend.Enums;
using MockHackothonBackend.Helpers;
using MockHackothonBackend.Models;


namespace MockHackothonBackend.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly JwtHelper _jwtHelper;

        public AuthService(AppDbContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        public async Task<AuthResponseDto?> Register(RegisterDto dto)
        {
            // Check if user exists
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return null;

            // Parse role
            if (!Enum.TryParse<Role>(dto.Role, true, out var role))
                return null;

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Create user
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = passwordHash,
                Role = role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate token
            var token = _jwtHelper.GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Role = user.Role.ToString(),
                Name = user.Name
            };
        }

        public async Task<AuthResponseDto?> Login(LoginDto dto)
        {
            // Find user
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return null;

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return null;

            // Generate token
            var token = _jwtHelper.GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Role = user.Role.ToString(),
                Name = user.Name
            };
        }
    }
}
