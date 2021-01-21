######################################################
#
# Dockerfile used to build a .NET Core application
# on either Windows Server Nano or Linux. 
# It uses a multi-stage build approach to reduce the 
# final image size.
#
#######################################################

# STEP 1: start from the .NET Core SDK base image and build the project
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

WORKDIR /app

# Copy just the NuGet dependency definition files (*.sln and *.csproj)
# and then run NuGet package restore. This approach maximizes the odds
# of using Docker's layer caching and not longer re-installing
# dependencies when the dependency definition files don't change.
COPY *.sln .
COPY Shop.API/Shop.API.csproj ./Shop.API/
COPY Shop.Data/Shop.Data.csproj ./Shop.Data/
COPY Shop.Service/Shop.Service.csproj ./Shop.Service/
COPY Shop.Web/Shop.Web.csproj ./Shop.Web/

RUN dotnet restore

# Then copy all the source files and build the project.
COPY . .
WORKDIR /app/Shop.API
RUN dotnet publish -c Release -o out
# STEP 2: start from the .NET Core runtime base and copy just the compiled assets to minimize image size
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
LABEL maintainer="madalina.cirstea97@gmail.com"

WORKDIR /app
COPY --from=build /app/Shop.API/out ./

# Make sure the application is loaded on the embedded webserver
ENTRYPOINT ["dotnet", "Shop.API.dll"]
