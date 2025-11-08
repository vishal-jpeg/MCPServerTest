# Use the official .NET 8 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything and build the app
COPY . .
RUN dotnet publish -c Release -o out

# Use the .NET 8 runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Expose port 8080 (Render expects apps to listen on this port)
EXPOSE 8080

# Run your .NET app
ENTRYPOINT ["dotnet", "McpDemo.dll"]
