using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Cinema.Helpers;

public enum UserRole
{
    /// <summary>
    ///     普通用户
    /// </summary>
    User,

    /// <summary>
    ///     影院管理员
    /// </summary>
    CinemaAdmin,

    /// <summary>
    ///     系统管理员
    /// </summary>
    SysAdmin
}

public class JwtHelper
{
    private readonly IConfiguration _configuration;

    public JwtHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    ///     创建一个JWT令牌
    /// </summary>
    /// <param name="userName">用户名</param>
    /// <param name="role">用户等级，参考上方注释</param>
    /// <returns>一个签发的JWT令牌</returns>
    public string GenerateToken(string userName, UserRole role)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.Role, role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "SampleKey"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"]));
        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            DateTime.Now,
            expires,
            credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    ///     从HTTP上下文解析用户的用户名
    /// </summary>
    /// <param name="httpContextAccessor">HTTP上下文</param>
    /// <returns>用户名</returns>
    public static string? SolveName(IHttpContextAccessor httpContextAccessor)
    {
        var identity = httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
        return identity?.FindFirst(ClaimTypes.Name)?.Value;
    }
}