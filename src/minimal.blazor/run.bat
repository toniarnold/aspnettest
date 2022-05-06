ECHO %ASPNETCORE_ENVIRONMENT%
:Start
CD .\bin\Release\net6.0\publish\
minimal.blazor.exe
GOTO Start
