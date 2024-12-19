using Integrity.Banking.Api;
using Integrity.Banking.Api.Models;
using Integrity.Banking.Application;
using Integrity.Banking.Domain;
using Integrity.Banking.Domain.Models;
using Integrity.Banking.Domain.Models.Config;
using Integrity.Banking.Domain.Repositories;
using Integrity.Banking.Infrastructure.Database;
using Integrity.Banking.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Scalar.AspNetCore;

var policyName = "Integrity";

var builder = WebApplication.CreateBuilder(args);

//builder.AddServiceDefaults();

//builder.AddMySqlDataSource("mysqldb"); // .NET Aspire

var dbConfig = builder.Configuration.GetSection("Database").Get<DbConfig>() ?? new();
var authzConfig = builder.Configuration.GetSection("Authorization").Get<AuthzConfig>() ?? new();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy(policyName,
                      policy =>
                      {
                          var origins = builder.Configuration.GetSection("Cors:Origins")?.Get<string[]>() ?? [];

                          policy.WithOrigins(origins)
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials();
                      });
});

builder.Services
    .AddAuthorization(options =>
    {
        options.AddPolicy(policyName, policy => policy.Requirements.Add(new ApiKeyRequirement()));
    })
    .AddHttpContextAccessor()
    .AddSingleton(authzConfig)
    .AddSingleton<IAuthorizationHandler, ApiKeyAuthorizationHandler>();

builder.Services
    .AddSingleton(dbConfig)
    .AddDbContext<BankingDbContext>()
    .AddScoped<IBankingRepository, BankingRepository>()
    .AddTransient<DepositHandler>()
    .AddTransient<WithdrawalHandler>()
    .AddTransient<AccountHandler>()
    .AddTransient<DepositService>()
    .AddTransient<WithdrawalService>()
    .AddTransient<AccountService>();

var app = builder.Build();

//app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors(policyName);
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapPost("/deposit", async Task<Results<BadRequest<TransactionResponse>, Ok<TransactionResponse>>>(TransactionRequest request) =>
{
    using var scope = app.Services.CreateScope();
    var bankingService = scope.ServiceProvider.GetRequiredService<DepositService>();

    var data = await bankingService.ProcessDepositAsync(new TransactionData
    {
        AccountId = request.AccountId,
        Amount = request.Amount,
        CustomerId = request.CustomerId
    });

    var response = new TransactionResponse();

    if (data != null)
    {
        response.CustomerId = data.CustomerId;
        response.AccountId = data.AccountId;
        response.Balance = data.Amount;
        response.Succeeded = true;

        return TypedResults.Ok(response);
    }

    return TypedResults.BadRequest(response);
}).RequireAuthorization(policyName);

app.MapPost("/withdrawal", async Task<Results<BadRequest<TransactionResponse>, Ok<TransactionResponse>>> (TransactionRequest request) =>
{
    using var scope = app.Services.CreateScope();
    var bankingService = scope.ServiceProvider.GetRequiredService<WithdrawalService>();

    var data = await bankingService.ProcessWithdrawalAsync(new TransactionData
    {
        AccountId = request.AccountId,
        Amount = request.Amount,
        CustomerId = request.CustomerId
    });

    var response = new TransactionResponse();

    if (data != null)
    {
        response.CustomerId = data.CustomerId;
        response.AccountId = data.AccountId;
        response.Balance = data.Amount;
        response.Succeeded = true;

        return TypedResults.Ok(response);
    }

    return TypedResults.BadRequest(response);
});

app.MapPost("/close-account", async Task<Results<BadRequest<CloseAccountResponse>, Ok<CloseAccountResponse>>> (CloseAccountRequest request) =>
{
    using var scope = app.Services.CreateScope();
    var bankingService = scope.ServiceProvider.GetRequiredService<AccountService>();

    var data = await bankingService.ProcessCloseAccountAsync(new CloseAccountData
    {
        CustomerId = request.CustomerId,
        AccountId  = request.AccountId,
    });

    var response = new CloseAccountResponse();

    if (data != null)
    {
        response.AccountId = data.AccountId;
        response.CustomerId = data.CustomerId;
        response.Succeeded = true;

        return TypedResults.Ok(response);
    }

    return TypedResults.BadRequest(response);
});

app.MapPost("/open-account", async Task<Results<BadRequest<OpenAccountResponse>, Ok<OpenAccountResponse>>> (OpenAccountRequest request) =>
{
    using var scope = app.Services.CreateScope();
    var bankingService = scope.ServiceProvider.GetRequiredService<AccountService>();

    var data = await bankingService.ProcessOpenAccountAsync(new OpenAccountData
    {
        CustomerId = request.CustomerId,
        AccountTypeId = request.AccountTypeId,
        Amount = request.InitialDeposit
    });

    var response = new OpenAccountResponse();

    if (data != null)
    {
        response.CustomerId = data.CustomerId;
        response.AccountTypeId = data.AccountTypeId;
        response.Balance = data.Amount;
        response.Succeeded = true;

        return TypedResults.Ok(response);
    }

    return TypedResults.BadRequest(response);
});

app.Run();