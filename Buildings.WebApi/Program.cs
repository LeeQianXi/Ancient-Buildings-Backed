using System.Security.Cryptography;
using Buildings.Infrastructure.Data;
using Buildings.Infrastructure.Repositories;
using Buildings.Infrastructure.Services;
using Buildings.Middleware;
using Buildings.Utils;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services
    .Configure<SnowflakeIdGeneratorOptions>(options => options.MachineId = 1)
    .AddSingleton<IIdGenerator<long>, SnowflakeIdGenerator>()
    .AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

#region DbContext

var connectionString = configuration["DATABASE_CONNECTION_STRING"];
if (connectionString is null)
{
    var path = configuration["DATABASE_CONNECTION_STRING_FILE"];
    if (File.Exists(path)) connectionString = File.ReadAllText(path);
}

if (connectionString is null)
    throw new ArgumentNullException(nameof(connectionString));
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.EnableDynamicJson();
var dataSource = dataSourceBuilder.Build();
builder.Services
    .AddDbContextFactory<BuildingDbContext>(options =>
        options.UseNpgsql(dataSource)
    )
    .AddScoped<ISecureRepository, SecureRepository>()
    .AddScoped<IAccountRepository, AccountRepository>();

#endregion

builder.Services
    .AddValidatorsFromAssembly(typeof(Program).Assembly, includeInternalTypes: true);

builder.Services
    .AddProblemDetails()
    .AddExceptionHandler<ValidationExceptionHandler>()
    .AddExceptionHandler<GlobalExceptionHandler>();

builder.Services
    .AddOpenApi("v1")
    .AddControllers();

builder.Services
    .Configure<JwtBearerOptions>(
        JwtBearerDefaults.AuthenticationScheme,
        options =>
        {
            const string audience = "AcBu";
            options.Audience = audience;
            options.RequireHttpsMetadata = builder.Environment.IsDevelopment();
            options.IncludeErrorDetails = !builder.Environment.IsDevelopment();

            var path = configuration["PUBLIC_KEY_FILE"] ??
                       throw new ArgumentException("PublicKey file path is required.");
            var publicKeyPem = File.ReadAllText(Path.GetFullPath(path));
            var rsa = RSA.Create();
            rsa.ImportFromPem(publicKeyPem);

            const string authority = "https://localhost";
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = authority,
                ValidateAudience = true,
                ValidAudience = audience,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1),

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new RsaSecurityKey(rsa)
            };
        })
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();
builder.Services
    .Configure<TokenServiceOptions>(
        JwtBearerDefaults.AuthenticationScheme,
        options =>
        {
            options.PublicKeyFilePath = configuration["PUBLIC_KEY_FILE"] ??
                                        throw new ArgumentException("PublicKey file path is required.");
            options.PrivateKeyFilePath = configuration["PRIVATE_KEY_FILE"] ??
                                         throw new ArgumentException("PrivateKey file path is required.");
        })
    .AddSingleton<ITokenService, TokenService>();

var app = builder.Build();

// 异常处理
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler();
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1"); // 设置文档路径
    });
}

// HTTPS 重定向
if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();
// 静态文件（应放在路由之前，避免不必要的路由处理）
app.UseStaticFiles();
// 路由匹配（必须放在身份验证、授权等之前）
app.UseRouting();
// 跨域配置
app.UseCors();
// 身份验证（依赖路由，但必须在授权之前）
app.UseAuthentication();
// 授权
app.UseAuthorization();
// 自定义中间件
//app.UseMiddleware<MyCustomMiddleware>();
// API 终结点映射
app.MapControllers();

await app.RunAsync();