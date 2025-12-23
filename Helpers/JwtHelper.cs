using AppBoleteriaApi.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AppBoleteriaApi.Helpers
{
	public class JwtHelper
	{
		private readonly IConfiguration _config;

		public JwtHelper(IConfiguration config)
		{
			_config = config;
		}

		public string GenerateToken(User user)
		{
			var claims = new[]
			{
				new Claim("id", user.Id.ToString()),
				new Claim("email", user.Email),
				new Claim("role", user.RoleId.ToString())
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _config["Jwt:Issuer"],
				audience: _config["Jwt:Audience"],
				claims: claims,
				expires: DateTime.Now.AddHours(8),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
