dotnet tool update --global dotnet-ef --version 10.0.9
dotnet build
dotnet ef --startup-project ../Minimal.Api/ database update --context ApplicationDbContext
pause