dotnet tool update --global dotnet-ef --version 7.0.3
dotnet build
dotnet ef --startup-project ../Minimal.Api/ database update --context ApplicationDbContext
pause