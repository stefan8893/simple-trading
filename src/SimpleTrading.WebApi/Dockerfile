FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BuildConfiguration=Release
ARG Version=0.0.0
WORKDIR /src

COPY src/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done

RUN dotnet restore "src/SimpleTrading.WebApi/SimpleTrading.WebApi.csproj"
COPY . .
WORKDIR "src/SimpleTrading.WebApi"
RUN dotnet build "./SimpleTrading.WebApi.csproj" -c $BuildConfiguration -Property:Version=$Version -o /app/build

FROM build AS publish
ARG BuildConfiguration=Release
ARG Version=0.0.0
RUN dotnet publish "./SimpleTrading.WebApi.csproj" -c $BuildConfiguration -Property:Version=$Version -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleTrading.WebApi.dll"]