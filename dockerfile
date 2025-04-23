# Stage 1: Build the application and use EF tools
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy nuget.config and project files
COPY nuget.config ./
COPY NewsPage.sln ./
COPY *.csproj ./

# Install EF CLI tools
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Restore dependencies
RUN dotnet restore NewsPage.csproj --verbosity detailed > restore.log 2>&1

# Copy all source code
COPY . ./

# Optional: Run EF migrations (uncomment and change names if needed)
# RUN dotnet ef migrations add InitialCreate -p NewsPage.csproj -s NewsPage.csproj --output-dir Migrations
# RUN dotnet ef database update -p NewsPage.csproj -s NewsPage.csproj

# Publish the application
RUN dotnet publish NewsPage.csproj -c Release -o /app/publish

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5288
ENV ASPNETCORE_URLS=http://+:5288

ENTRYPOINT ["dotnet", "NewsPage.dll"]
