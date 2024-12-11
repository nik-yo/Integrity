using Integrity.Banking.Application;
using Integrity.Banking.Domain.Models;
using Integrity.Banking.Domain.Repositories;
using Integrity.Banking.Infrastructure.Database;
using Integrity.Banking.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Scalar.AspNetCore;

var policyName = "Integrity";

var builder = WebApplication.CreateBuilder(args);

//builder.AddServiceDefaults();

builder.AddMySqlDataSource("mysqldb"); // .NET Aspire

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
    .AddDbContext<BankingDbContext>()
    .AddScoped<IBankingRepository, BankingRepository>()
    .AddTransient<BankingService>();

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

app.MapPost("/deposit", async Task<Results<BadRequest<TransactionResponse>, Ok<TransactionResponse>>>(TransactionRequest request) =>
{
    using var scope = app.Services.CreateScope();
    var bankingService = scope.ServiceProvider.GetRequiredService<BankingService>();

    var response = await bankingService.MakeDepositAsync(request);

    if (response.Succeeded)
    {
        return TypedResults.Ok(response);
    }

    return TypedResults.BadRequest(response);
});

app.MapPost("/withdrawal", async Task<Results<BadRequest<TransactionResponse>, Ok<TransactionResponse>>> (TransactionRequest request) =>
{
    using var scope = app.Services.CreateScope();
    var bankingService = scope.ServiceProvider.GetRequiredService<BankingService>();

    var response = await bankingService.MakeWithdrawalAsync(request);

    if (response.Succeeded)
    {
        return TypedResults.Ok(response);
    }

    return TypedResults.BadRequest(response);
});

app.MapPost("/close-account", async Task<Results<BadRequest<CloseAccountResponse>, Ok<CloseAccountResponse>>> (CloseAccountRequest request) =>
{
    using var scope = app.Services.CreateScope();
    var bankingService = scope.ServiceProvider.GetRequiredService<BankingService>();

    var response = await bankingService.CloseAccountAsync(request);

    if (response.Succeeded)
    {
        return TypedResults.Ok(response);
    }

    return TypedResults.BadRequest(response);
});

app.MapPost("/open-account", async Task<Results<BadRequest<OpenAccountResponse>, Ok<OpenAccountResponse>>> (OpenAccountRequest request) =>
{
    using var scope = app.Services.CreateScope();
    var bankingService = scope.ServiceProvider.GetRequiredService<BankingService>();

    var response = await bankingService.OpenAccountAsync(request);

    if (response.Succeeded)
    {
        return TypedResults.Ok(response);
    }

    return TypedResults.BadRequest(response);
});

app.Run();