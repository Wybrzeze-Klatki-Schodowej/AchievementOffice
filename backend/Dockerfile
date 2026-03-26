FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY ["AchievementOffice.sln", "./"]
COPY ["AchievementOffice/AchievementOffice.csproj", "AchievementOffice/"]

RUN dotnet restore
COPY . .
RUN dotnet publish "AchievementOffice/AchievementOffice.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "AchievementOffice.dll"]