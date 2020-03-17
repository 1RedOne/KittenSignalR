FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
COPY /bin/Release/netcoreapp3.0/publish/ app/
COPY /bin/Release/netcoreapp3.0/publish/wwwroot/* app/wwwroot/
EXPOSE 80 443 5000 5001
WORKDIR app
ENTRYPOINT ["dotnet", "KittenSignalR.dll"]