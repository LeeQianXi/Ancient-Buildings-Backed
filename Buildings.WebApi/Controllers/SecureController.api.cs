using Buildings.Commands.Security;
using Buildings.Infrastructure.Repositories;
using Buildings.Infrastructure.Services;
using Buildings.Responses.Secure;
using Buildings.Utils;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buildings.Controllers;

[Authorize]
[ApiController]
[Route("/api/v1/[controller]")]
[Tags("Authentication")]
public class SecureController(
    ILogger<SecureController> logger,
    ISecureRepository secureRepository,
    IAccountRepository accountRepository
) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    [EndpointSummary("注册账号")]
    [ProducesResponseType<AuthRegisterResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Register(
        [FromBody] AuthRegisterCommand command,
        [FromServices] IValidator<AuthRegisterCommand> validator,
        [FromServices] IPasswordHasher passwordHasher
    )
    {
        var validate = await validator.ValidateAsync(command);
        if (!validate.IsValid) throw new ValidationException(validate.Errors);
        if (await secureRepository.ExistsAccountAsync(command.Email)) return Unauthorized("Email already registered");
        var passwdHash = passwordHasher.SaltedHash(command.Password);
        var account = await secureRepository.InsertAccountAsync(command.Email, passwdHash, command.Username);
        await accountRepository.InitializeUserAsync(account);
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
    [EndpointSummary("帐号密码登陆账号")]
    [ProducesResponseType<AuthLoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> LoginAsync(
        [FromBody] AuthLoginCommand command,
        [FromServices] IValidator<AuthLoginCommand> validator,
        [FromServices] IPasswordHasher passwordHasher,
        [FromServices] ITokenService tokenService)
    {
        var validate = await validator.ValidateAsync(command);
        if (!validate.IsValid) throw new ValidationException(validate.Errors);
        if (!await secureRepository.ExistsAccountAsync(command.Email)) return NotFound("Email doesn't registered.");
        var (userId, passwordSaltHash) = await secureRepository.GetAccountVerifyAsync(command.Email);
        if (!passwordHasher.Verify(command.Password, passwordSaltHash)) return Unauthorized("Invalid password");
        // 生成访问令牌和刷新令牌
        var (accessToken, accessTokenExpiry) = tokenService.GenerateAccessToken(userId, command.Email, command.Hash);
        var (refreshToken, refreshTokenExpiry) = tokenService.GenerateRefreshToken();
        await secureRepository.CacheRefreshTokenAsync(userId, command.Hash, refreshToken, refreshTokenExpiry);
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
    [HttpPost("refresh")]
    [EndpointSummary("刷新AccessToken")]
    [ProducesResponseType<RefreshTokenResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshTokenAsync(
        [FromBody] RefreshTokenCommand command,
        [FromServices] IValidator<RefreshTokenCommand> validator,
        [FromServices] ITokenService tokenService
    )
    {
        var validate = await validator.ValidateAsync(command);
        if (!validate.IsValid) throw new ValidationException(validate.Errors);
        if (!await secureRepository.ValidateRefreshTokenAsync(command.UserId, command.Hash, command.RefreshToken))
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

    [HttpPost("changeEmail")]
    [EndpointSummary("修改账号绑定邮箱")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeEmailAsync(
        [FromBody] AuthChangeEmailCommand command,
        IValidator<AuthChangeEmailCommand> validator)
    {
        var validate = await validator.ValidateAsync(command);
        if (!validate.IsValid) throw new ValidationException(validate.Errors);
        if (!await secureRepository.ExistsAccountAsync(command.UserId))
            return NotFound("User doesn't exist.");
        await secureRepository.ChangeEmailAsync(command.UserId, command.NewEmail);
        return Ok();
    }

    [HttpPost("changePassword")]
    [EndpointSummary("修改账号密码")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePasswordAsync(
        [FromBody] AuthChangePasswordCommand command,
        IValidator<AuthChangePasswordCommand> validator)
    {
        var validate = await validator.ValidateAsync(command);
        if (!validate.IsValid) throw new ValidationException(validate.Errors);
        if (await secureRepository.ExistsAccountAsync(command.UserId))
            return NotFound("User doesn't exist.");
        //TODO: Validate Cofirm key
        await secureRepository.ChangePasswordAsync(command.UserId, command.NewPassword);
        return Ok();
    }

    [HttpGet("changePassword")]
    [EndpointSummary("获取修改账号密码单次临时验证码")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangePasswordAsync([FromHeader] long userId)
    {
        if (await secureRepository.ExistsAccountAsync(userId))
            return NotFound("User doesn't exist.");
        //TODO: Deal Confirm Key
        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("resetPassword")]
    [EndpointSummary("重置账号密码")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ResetPasswordAsync(
        [FromBody] AuthResetPasswordCommand command,
        IValidator<AuthResetPasswordCommand> validator,
        [FromServices] IPasswordHasher passwordHasher)
    {
        var validate = await validator.ValidateAsync(command);
        if (!validate.IsValid) throw new ValidationException(validate.Errors);
        if (!await secureRepository.ExistsAccountAsync(command.Email))
            return NotFound("User doesn't exist.");
        //TODO: Validate Confirm key
        var (userId, hash) = await secureRepository.GetAccountVerifyAsync(command.Email);
        if (!passwordHasher.Verify(command.Password, hash))
            return Unauthorized("Invalid Password");
        await secureRepository.ChangePasswordAsync(userId, passwordHasher.SaltedHash(command.Password));
        return Ok();
    }

    [AllowAnonymous]
    [HttpGet("resetPassword")]
    [EndpointSummary("获取重置账号单次临时验证码")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResetPasswordAsync([FromHeader] string email)
    {
        if (await secureRepository.ExistsAccountAsync(email))
            return NotFound("User doesn't exist.");
        //TODO: Deal Confirm Key
        return NoContent();
    }
}