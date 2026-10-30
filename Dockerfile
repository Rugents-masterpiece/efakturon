FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5050
EXPOSE 5051

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Efakturon.sln", "."]
COPY ["src/Efakturon.Web/Efakturon.Web.csproj", "src/Efakturon.Web/"]
COPY ["src/Efakturon.Application/Efakturon.Application.csproj", "src/Efakturon.Application/"]
COPY ["src/Efakturon.Infrastructure/Efakturon.Infrastructure.csproj", "src/Efakturon.Infrastructure/"]
COPY ["src/Efakturon.Domain/Efakturon.Domain.csproj", "src/Efakturon.Domain/"]
COPY ["tests/Efakturon.Tests/Efakturon.Tests.csproj", "tests/Efakturon.Tests/"]
RUN dotnet restore "Efakturon.sln"
COPY . .
WORKDIR "/src/src/Efakturon.Web"
RUN dotnet build "Efakturon.Web.csproj" -c Release -o /app/build
RUN dotnet publish "Efakturon.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Efakturon.Web.dll"]
