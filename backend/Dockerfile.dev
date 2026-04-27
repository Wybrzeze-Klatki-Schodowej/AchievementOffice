FROM mcr.microsoft.com/dotnet/sdk:10.0

WORKDIR /app

COPY AchievementOffice/*.csproj ./AchievementOffice/
RUN dotnet restore ./AchievementOffice/AchievementOffice.csproj

ENTRYPOINT ["dotnet", "watch", "run", "--project", "AchievementOffice/AchievementOffice.csproj", "--urls", "http://0.0.0.0:8080"]