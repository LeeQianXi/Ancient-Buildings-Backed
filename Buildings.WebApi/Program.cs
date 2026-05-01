using System.Security.Cryptography;
using Buildings.Infrastructure.Data;
using Buildings.Middleware;
using Buildings.Utils;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services
    .Configure<SnowflakeIdGeneratorOptions>(options => options.MachineId = 1)
    .AddSingleton<IIdGenerator<long>, SnowflakeIdGenerator>()
    .AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

builder.Services.AddDbContextFactory<BuildingDbContext>(dbBuilder =>
{
    var connectionString = configuration["SQLSERVER_CONNECTION_STRING"];
    if (connectionString is null)
    {
        var path = configuration["SQLSERVER_CONNECTION_STRING_FILE"];
        if (File.Exists(path)) connectionString = File.ReadAllText(path);
    }

    if (connectionString is null)
        throw new ArgumentNullException(nameof(connectionString));
    dbBuilder.UseSqlServer(connectionString);
});

builder.Services
    .AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services
    .AddProblemDetails()
    .AddExceptionHandler<GlobalExceptionHandler>();

builder.Services
    .AddControllers();

builder.Services
    .Configure<JwtBearerOptions>(options =>
    {
        const string authority = "https://localhost";
        options.Authority = authority;
        const string audience = "AcBu";
        options.Audience = audience;
        options.RequireHttpsMetadata = builder.Environment.IsDevelopment();
        options.IncludeErrorDetails = !builder.Environment.IsDevelopment();
        var path = configuration["PUBLIC_KEY_FILE"];
        var publicKeyPem = File.ReadAllText(Path.GetFullPath(path!));
        var rsa = RSA.Create();
        rsa.ImportFromPem(publicKeyPem);
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

// HTTPS 重定向
app.UseHttpsRedirection();
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