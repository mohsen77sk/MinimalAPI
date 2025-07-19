dotnet tool update --global dotnet-ef --version 9.0.7
dotnet build
dotnet ef --startup-project ../Minimal.Api/ database update --context ApplicationDbContext
pause