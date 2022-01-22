FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["API.ENSEK/API.ENSEK.csproj", "API.ENSEK/"]
RUN dotnet restore "API.ENSEK/API.ENSEK.csproj"
COPY . .
WORKDIR "/src/API.ENSEK"
RUN dotnet build "API.ENSEK.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "API.ENSEK.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.ENSEK.dll"]
