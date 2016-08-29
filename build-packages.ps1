cd .\.nuget

.\nuget.exe pack ..\src\EPiBootstrapArea\EPiBootstrapArea.csproj -Properties Configuration=Release
.\nuget.exe pack ..\src\EPiBootstrapArea.Forms\EPiBootstrapArea.Forms.csproj -Properties Configuration=Release
cd ..\