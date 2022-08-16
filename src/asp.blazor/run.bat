ECHO %ASPNETCORE_ENVIRONMENT%
::VCTargetsPath required without --no-build in the VS command prompt:
::SET VCTargetsPath=C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Microsoft\VC\v170\
:Start
dotnet run --no-build
GOTO Start
