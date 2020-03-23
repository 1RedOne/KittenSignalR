#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
RUN apt-get update
RUN apt-get install -y python3 
RUN curl -L https://yt-dl.org/downloads/latest/youtube-dl -o /usr/local/bin/youtube-dl
RUN chmod a+rx /usr/local/bin/youtube-dl
RUN ln -s /usr/bin/python3 /usr/local/bin/python
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["KittenSignalR/KittenSignalR.csproj", "KittenSignalR/"]
RUN dotnet restore "KittenSignalR/KittenSignalR.csproj"
COPY . .
WORKDIR "/src/KittenSignalR"
RUN dotnet build "KittenSignalR.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KittenSignalR.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KittenSignalR.dll"]