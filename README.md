# Banking API

## Setup (Windows)

Run the following commands in terminal:

1. Install Docker Desktop: **winget install Docker.DockerDesktop**
2. Install MySql Workbench: **winget install Oracle.MySQLWorkbench**
3. Install EF CLI tool: **dotnet tool install --global dotnet-ef**

4. Run .NET Aspire from Visual Studio or use the following command:
   
   ** dotnet run --project ./api/Integrity.Aspire/Integrity.Aspire.AppHost**

5. Change directory to Integrity.Banking.Infrastructure folder and run: **dotnet ef database update** 

## Sending Request

1. Run .NET Aspire from Visual Studio or use the following command:
   
   ** dotnet run --project ./api/Integrity.Aspire/Integrity.Aspire.AppHost**

2. Visit: https://localhost:7282/scalar/v1

## Links
https://docs.google.com/document/d/1qMJer3w3KhQji_YLILLASKIV27cZkr51rOjYFI8tDOw/edit?usp=sharing

## Consideration
1. Fail transaction if making a deposit or withdrawal on closed account
2. Authorization required
3. Integer for id is bad practice or at least needs to be masked on client side
4. Response data are not defined, need to ensure not wrong data is returned
5. When writing back to database, there's redundant db call to make sure that the transaction is still valid. eg. when making deposit, ensure that the account still belongs to the customer in the case it is called independently.
6. It's probably better to be backed by queue