#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat


FROM mcr.microsoft.com/dotnet/core/runtime:2.2-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["src/Ventura.Cli/Ventura.Cli.csproj", "src/Ventura.Cli/"]
RUN dotnet restore "src/Ventura.Cli/Ventura.Cli.csproj"
COPY . .
WORKDIR "src/Ventura.Cli"
RUN dotnet build "Ventura.Cli.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Ventura.Cli.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Ventura.Cli.dll"]