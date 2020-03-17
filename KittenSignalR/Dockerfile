FROM mcr.microsoft.com/dotnet/core/aspnet:3.0
COPY /bin/Release/netcoreapp3.0/publish/ app/
EXPOSE 80 443
ENTRYPOINT ["dotnet", "app/KittenSignalR.dll"]