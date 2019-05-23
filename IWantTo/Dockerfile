FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY IWantTo.csproj IWantTo/
RUN dotnet restore IWantTo/IWantTo.csproj
WORKDIR /src/IWantTo
COPY . .
RUN dotnet build IWantTo.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish IWantTo.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "IWantTo.dll"]
