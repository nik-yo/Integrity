var builder = DistributedApplication.CreateBuilder(args);

var password = builder.AddParameter("password", secret: true);

var mysql = builder.AddMySql("mysql", password, port: 3306)
                .WithDataVolume()
                .WithLifetime(ContainerLifetime.Persistent);
var mysqldb = mysql.AddDatabase("banking");

builder.AddProject<Projects.Integrity_Banking_Api>("integrity-banking-api")
    .WithReference(mysqldb);

builder.Build().Run();
