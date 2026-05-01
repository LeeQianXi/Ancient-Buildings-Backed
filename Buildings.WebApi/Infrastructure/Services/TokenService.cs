using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Buildings.Infrastructure.Services;

/// <summary>
///     令牌管理服务
/// </summary>
public interface ITokenService
{
    /// <summary>
    ///     生成访问令牌
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="email">用户邮箱</param>
    /// <param name="fingerprinting">用户指纹</param>
    /// <returns>访问令牌</returns>
    (string AccessToken, DateTimeOffset AccessTokenExpiry) GenerateAccessToken(long userId, string email,
        string fingerprinting);

    /// <summary>
    ///     生成刷新令牌
    /// </summary>
    /// <returns>刷新令牌</returns>
    (string RefreshToken, DateTimeOffset RefreshTokenExpiry) GenerateRefreshToken();

    /// <summary>
    ///     从令牌中获取用户ID
    /// </summary>
    /// <param name="accessToken">访问令牌</param>
    /// <returns>用户ID</returns>
    /// <exception cref="InvalidOperationException">令牌无效时抛出</exception>
    long GetUserIdFromAccessToken(string accessToken);
}

/// <summary>
///     令牌管理服务实现
/// </summary>
public class TokenService : ITokenService
{
    private readonly JwtBearerOptions _jwtBearerOptions;

    private readonly RsaSecurityKey _signingKey;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();
    private readonly RsaSecurityKey _validationKey;

    /// <summary>
    ///     令牌管理服务实现
    /// </summary>
    public TokenService(ILogger<TokenService> logger,
        IConfiguration configuration,
        IOptionsMonitor<JwtBearerOptions> jwtBearerOptions,
        IOptionsMonitor<TokenServiceOptions> tokenServiceOptions)
    {
        _jwtBearerOptions = jwtBearerOptions.Get(JwtBearerDefaults.AuthenticationScheme);
        var tokenOptions = tokenServiceOptions.Get(JwtBearerDefaults.AuthenticationScheme);
        AccessTokenExpirationMinutes = tokenOptions.AccessTokenExpirationMinutes;
        RefreshTokenExpirationDays = tokenOptions.RefreshTokenExpirationDays;
        {
            var privateKeyPath = tokenOptions.PrivateKeyFilePath
                                 ?? throw new InvalidOperationException("Private key file path is missing.");
            var privateKeyPem = File.ReadAllText(Path.GetFullPath(privateKeyPath));
            var rsaPrivate = RSA.Create();
            rsaPrivate.ImportFromPem(privateKeyPem);
            _signingKey = new RsaSecurityKey(rsaPrivate);
        }
        {
            var publicKeyPath = tokenOptions.PublicKeyFilePath
                                ?? throw new InvalidOperationException("Public key file path is missing.");
            var publicKeyPem = File.ReadAllText(Path.GetFullPath(publicKeyPath));
            var rsaPublic = RSA.Create();
            rsaPublic.ImportFromPem(publicKeyPem);
            _validationKey = new RsaSecurityKey(rsaPublic);
        }
    }

    public int AccessTokenExpirationMinutes { get; }
    public int RefreshTokenExpirationDays { get; }

    public (string AccessToken, DateTimeOffset AccessTokenExpiry) GenerateAccessToken(long userId, string email,
        string fingerprinting)
    {
        var issuer = _jwtBearerOptions.TokenValidationParameters.ValidIssuer
                     ?? throw new InvalidOperationException("JWT issuer is not configured.");
        var audience = _jwtBearerOptions.TokenValidationParameters.ValidAudience
                       ?? throw new InvalidOperationException("JWT audience is not configured.");
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.AtHash, fingerprinting),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var credentials = new SigningCredentials(_signingKey, SecurityAlgorithms.RsaSha256);
        var expires = DateTime.UtcNow.AddMinutes(AccessTokenExpirationMinutes);
        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            DateTime.UtcNow,
            expires,
            credentials
        );
        var accessToken = _tokenHandler.WriteToken(token);
        return (accessToken, new DateTimeOffset(expires));
    }

    /// <summary>
    ///     生成刷新令牌
    /// </summary>
    /// <returns>刷新令牌</returns>
    public (string RefreshToken, DateTimeOffset RefreshTokenExpiry) GenerateRefreshToken()
    {
        var refreshToken = Guid.NewGuid().ToString();
        var expires = DateTimeOffset.UtcNow.AddDays(RefreshTokenExpirationDays);
        return (refreshToken, expires);
    }

    /// <summary>
    ///     从令牌中获取用户ID
    /// </summary>
    /// <param name="accessToken">访问令牌</param>
    /// <returns>用户ID</returns>
    /// <exception cref="InvalidOperationException">令牌无效时抛出</exception>
    public long GetUserIdFromAccessToken(string accessToken)
    {
        try
        {
            // 克隆验证参数以避免修改原始配置
            var validationParameters = _jwtBearerOptions.TokenValidationParameters.Clone();
            // 确保使用公钥进行验证（实际上原始配置中的 IssuerSigningKey 已经是公钥）
            validationParameters.IssuerSigningKey = _validationKey;
            validationParameters.ValidateLifetime = true;
            // 建议与 JwtBearerOptions 中配置的 ClockSkew 保持一致，此处默认 1 分钟
            validationParameters.ClockSkew = TimeSpan.FromMinutes(1);

            var principal = _tokenHandler.ValidateToken(accessToken, validationParameters, out _);
            var userIdClaim = principal.FindFirst("userId") ?? principal.FindFirst(JwtRegisteredClaimNames.Sub);
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
                throw new InvalidOperationException("User ID claim missing or invalid.");

            return userId;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Invalid access token.", ex);
        }
    }
}

[Serializable]
public class TokenServiceOptions
{
    public const int DefaultAccessTokenExpirationMinutes = 15;
    public const int DefaultRefreshTokenExpirationDays = 7;
    public int AccessTokenExpirationMinutes { get; set; } = DefaultAccessTokenExpirationMinutes;
    public int RefreshTokenExpirationDays { get; set; } = DefaultRefreshTokenExpirationDays;
    public string PublicKeyFilePath { get; set; } = string.Empty;
    public string PrivateKeyFilePath { get; set; } = string.Empty;
}