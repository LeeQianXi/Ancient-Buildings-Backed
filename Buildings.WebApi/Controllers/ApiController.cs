using Buildings.Infrastructure.Data;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Buildings.Controllers;

[ApiController]
[Route("[controller]")]
public partial class ApiController(
    ILogger<ApiController> logger,
    IDbContextFactory<BuildingDbContext> dbContextFactory
) : ControllerBase
{
    private static readonly IValidator<string> EmailValidator;

    static ApiController()
    {
        var validator = new InlineValidator<string>();
        validator.RuleFor(x => x)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is not valid");
        EmailValidator = validator;
    }
}