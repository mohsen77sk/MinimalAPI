dotnet tool update --global dotnet-ef --version 8.0.0
dotnet build
dotnet ef --startup-project ../Minimal.Api/ database update --context ApplicationDbContext
pause