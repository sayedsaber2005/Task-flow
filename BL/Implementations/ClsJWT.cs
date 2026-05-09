using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ProjectManagement.BL.Interfaces;
using ProjectManagement.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectManagement.BL.Implementations
{
    public class ClsJWT : IJWT
    {
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClsJWT(IConfiguration config, UserManager<ApplicationUser> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        public async Task<string> GenerateToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                //new Claim("imageUrl", user.ProfileImageUrl ?? "")
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["JWT:Key"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    double.Parse(_config["JWT:DurationInMinutes"])
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

//         public TbRefreshToken GenerateRefreshToken()
//         {
//             var randomBytes = new byte[32];
//             using (var rng = RandomNumberGenerator.Create())
//             {
//                 rng.GetBytes(randomBytes);
//             }
//             return new TbRefreshToken
//             {
//                 Token = Convert.ToBase64String(randomBytes),
//                 ExpiresOn = DateTime.UtcNow.AddDays(7),
//                 CreatedOn = DateTime.UtcNow
//             };
//         }

//         public void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
//         {
//             var cookieOptions = new CookieOptions
//             {
//                 HttpOnly = true,
//                 Expires = expires.ToLocalTime()
//             };

//             var response = _httpContextAccessor?.HttpContext?.Response;
//             response?.Cookies.Append("refreshToken", refreshToken, cookieOptions);
//         }
    }
}
