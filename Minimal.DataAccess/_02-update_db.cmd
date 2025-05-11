dotnet tool update --global dotnet-ef --version 9.0.4
dotnet build
dotnet ef --startup-project ../Minimal.Api/ database update --context ApplicationDbContext
pause