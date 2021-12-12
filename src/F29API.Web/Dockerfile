FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["F29API.Web/F29API.Web.csproj", "F29API.Web/"]
RUN dotnet restore "F29API.Web/F29API.Web.csproj"
COPY . .
WORKDIR "/src/F29API.Web"
RUN dotnet build "F29API.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "F29API.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "F29API.Web.dll"]
