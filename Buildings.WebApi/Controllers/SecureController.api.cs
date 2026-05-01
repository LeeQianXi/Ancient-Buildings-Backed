using Buildings.Commands.Security;
using Buildings.Infrastructure.Repositories;
using Buildings.Infrastructure.Services;
using Buildings.Responses;
using Buildings.Utils;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buildings.Controllers;

[ApiController]
[Route("/api/secure")]
[Authorize]
public class SecureController(
    ILogger<SecureController> logger,
    IAccountRepository accountRepository
) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("register")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(AuthRegisterResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Register(
        [FromBody] AuthRegisterCommand command,
        IValidator<AuthRegisterCommand> validator,
        IPasswordHasher passwordHasher)
    {
        var validate = await validator.ValidateAsync(command);
        if (!validate.IsValid) throw new ValidationException(validate.Errors);
        if (await accountRepository.ExistsAccountAsync(command.Email)) return BadRequest("Email already registered");
        var passwdHash = passwordHasher.SaltedHash(command.Password);
        var account = await accountRepository.InsertAccountAsync(command.Email, passwdHash, command.Username);
        return Ok(new AuthRegisterResponse
        {
            UserId = account.UserId,
            Email = account.Email,
            Username = account.UserName,
            CreatedAt = account.CreatedAt
        });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(AuthLoginResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login(
        [FromBody] AuthLoginCommand command,
        IValidator<AuthLoginCommand> validator,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        var validate = await validator.ValidateAsync(command);
        if (!validate.IsValid) throw new ValidationException(validate.Errors);
        if (!await accountRepository.ExistsAccountAsync(command.Email)) return BadRequest("Email doesn't registered.");
        var (userId, passwordSaltHash) = await accountRepository.GetAccountVerifyAsync(command.Email);
        if (!passwordHasher.Verify(command.Password, passwordSaltHash)) return Unauthorized("Invalid password");
        // 生成访问令牌和刷新令牌
        var (accessToken, accessTokenExpiry) = tokenService.GenerateAccessToken(userId, command.Email, command.Hash);
        var (refreshToken, refreshTokenExpiry) = tokenService.GenerateRefreshToken();
        await accountRepository.CacheRefreshTokenAsync(userId, command.Hash, refreshToken, refreshTokenExpiry);
        return Ok(new AuthLoginResponse
        {
            UserId = userId,
            Email = command.Email,
            AccessToken = accessToken,
            AccessTokenExpire = accessTokenExpiry,
            RefreshToken = refreshToken,
            RefreshTokenExpire = refreshTokenExpiry
        });
    }

    [AllowAnonymous]
    [HttpGet("refresh")]
    [ProducesResponseType(typeof(RefreshTokenResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshTokenAsync(
        [FromBody] RefreshTokenCommand command,
        IValidator<RefreshTokenCommand> validator,
        ITokenService tokenService
    )
    {
        var validate = await validator.ValidateAsync(command);
        if (!validate.IsValid) throw new ValidationException(validate.Errors);
        if (!await accountRepository.ValidateRefreshTokenAsync(command.UserId, command.Hash, command.RefreshToken))
            return Unauthorized("Invalid Refresh Token");
        var (accessToken, accessTokenExpire) =
            tokenService.GenerateAccessToken(command.UserId, command.Email, command.Hash);
        return Ok(new RefreshTokenResponse
        {
            UserId = command.UserId,
            AccessToken = accessToken,
            AccessTokenExpire = accessTokenExpire
        });
    }

    public async Task<IActionResult> ChangeEmailAsync(
        [FromBody] ChangeAccountEmailCommand command,
        IValidator<ChangeAccountEmailCommand> validator)
    {
        var validate = await validator.ValidateAsync(command);
        if (!validate.IsValid) throw new ValidationException(validate.Errors);
        return Ok();
    }
}