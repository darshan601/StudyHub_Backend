FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app
EXPOSE 8080
EXPOSE 8081
COPY --chmod=755 wait-for-it.sh /app/wait-for-it.sh

USER $APP_UID


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["StudyHub.Presentation/StudyHub.Presentation.csproj", "StudyHub.Presentation/"]
COPY ["StudyHub.Infrastructure/StudyHub.Infrastructure.csproj", "StudyHub.Infrastructure/"]
COPY ["StudyHub.Core/StudyHub.Core.csproj", "StudyHub.Core/"]
RUN dotnet restore "StudyHub.Presentation/StudyHub.Presentation.csproj"
COPY . .
WORKDIR "/src/StudyHub.Presentation"
RUN dotnet build "StudyHub.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "StudyHub.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StudyHub.Presentation.dll"]
