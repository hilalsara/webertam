FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ErtamIK.WebPortal.csproj", "."]
RUN dotnet restore "ErtamIK.WebPortal.csproj"

COPY . .
RUN dotnet publish "ErtamIK.WebPortal.csproj" \
    --configuration Release \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ErtamIK.WebPortal.dll"]
