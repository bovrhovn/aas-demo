FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src

COPY ["aas.demo/aas.web.api.core", "aas.web.api.core/"]
RUN dotnet restore "aas.demo/aas.web.api.core.csproj"

COPY . .

WORKDIR "/src/"
RUN dotnet build "aas.demo/aas.web.api.core" -c Release -o /app

FROM build AS publish
RUN dotnet publish "aas.demo/aas.web.api.core" -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 as final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "aas.web.api.core.dll"]