dotnet build Source/CoreLibrary/BlackPearl.Controls.CoreLibrary.csproj --configuration Release --framework net48
dotnet build Source/CoreLibrary/BlackPearl.Controls.CoreLibrary.csproj --configuration Release --framework net6.0-windows7.0
dotnet pack Source/CoreLibrary/BlackPearl.Controls.CoreLibrary.csproj --output ./output -p:NuspecFile=BlackPearl.Controls.CoreLibrary.nuspec