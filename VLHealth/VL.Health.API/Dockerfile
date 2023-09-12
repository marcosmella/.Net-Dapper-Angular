FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /usr/lib/ssl/openssl.cnf
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["VL.Health.Service/VL.Health.Service.csproj", "VL.Health.Service/"]
COPY ["VL.Health.API/VL.Health.API.csproj", "VL.Health.API/"]
COPY ["VL.Health.DB/VL.Health.DB.csproj", "VL.Health.DB/"]
COPY ["VL.Health.Domain/VL.Health.Domain.csproj", "VL.Health.Domain/"]
COPY ["VL.Health.Infrastructure/VL.Health.Infrastructure.csproj", "VL.Health.Infrastructure/"]
COPY ["VL.Health.Interfaces/VL.Health.Interfaces.csproj", "VL.Health.Interfaces/"]
COPY ["VL.Health.IoC/VL.Health.IoC.csproj", "VL.Health.IoC/"]
COPY ["VL.Health.Service/nuget.config", "/src"]

RUN dotnet restore "VL.Health.Service/VL.Health.Service.csproj"
COPY . .
WORKDIR "/src/VL.Health.Service"
RUN dotnet build "VL.Health.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VL.Health.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VL.Health.Service.dll"]

