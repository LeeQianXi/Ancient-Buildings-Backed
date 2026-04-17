using Buildings.Infrastructure.Data;
using Buildings.Middleware;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

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