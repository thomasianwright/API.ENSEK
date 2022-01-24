FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WebApp.ENSEK/WebApp.ENSEK.csproj", "WebApp.ENSEK/"]
RUN dotnet restore "WebApp.ENSEK/WebApp.ENSEK.csproj"
COPY . .
WORKDIR "/src/WebApp.ENSEK"
RUN dotnet build "WebApp.ENSEK.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApp.ENSEK.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApp.ENSEK.dll"]
