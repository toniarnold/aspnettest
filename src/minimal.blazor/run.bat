ECHO %ASPNETCORE_ENVIRONMENT%
:Start
dotnet run --no-build --project minimal.blazor.csproj
GOTO Start
