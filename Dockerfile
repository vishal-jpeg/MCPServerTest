# Use the official .NET 9 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy everything and build the app
COPY . .
RUN dotnet publish -c Release -o out

# Use the .NET 9 runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .

# Expose port 8080 (Render uses this internally)
EXPOSE 8080

# Run the API
ENTRYPOINT ["dotnet", "McpDemo.dll"]
