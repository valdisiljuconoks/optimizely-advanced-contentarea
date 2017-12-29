cd .\.nuget

.\nuget.exe pack ..\src\EPiBootstrapArea\EPiBootstrapArea.csproj -Properties Configuration=Release -IncludeReferencedProjects
.\nuget.exe pack ..\src\EPiBootstrapArea.Forms\EPiBootstrapArea.Forms.csproj -Properties Configuration=Release -IncludeReferencedProjects
cd ..\