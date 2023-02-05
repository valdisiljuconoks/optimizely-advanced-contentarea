cd .\.nuget

cd .\..\src\AdvancedContentArea\
dotnet build -c Release
dotnet pack -c Release
dotnet pack --include-symbols -p:SymbolPackageFormat=snupkg
copy .\bin\Release\*.nupkg .\..\..\.nuget\
copy .\bin\Release\*.snupkg .\..\..\.nuget\

cd .\..\AdvancedContentArea.Forms\
dotnet build -c Release
dotnet pack -c Release
dotnet pack --include-symbols -p:SymbolPackageFormat=snupkg
copy .\bin\Release\*.nupkg .\..\..\.nuget\
copy .\bin\Release\*.snupkg .\..\..\.nuget\
cd .\..\..\
