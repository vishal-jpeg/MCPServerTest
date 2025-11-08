# ShopSmartMCP.API

Lightweight ASP.NET Core Web API that simulates a simple product search and order placement MCP (Marketplace Connector Protocol) demo.

## Quick facts
- Target framework: .NET8
- C# language version:12
- Controller: `ShopSmartMCP.API/Controllers/McpController.cs`
- Mock data: `ShopSmartMCP.API/mockdata/products.json`, `ShopSmartMCP.API/mockdata/orders.json`
- Schema file: `ShopSmartMCP.API/schema/mcp-schema.json`

## Requirements
- .NET8 SDK
- Visual Studio2022 (or later) or `dotnet` CLI

## Run (Visual Studio)
1. Open the solution in Visual Studio.
2. To change the URL used when pressing F5, edit `ShopSmartMCP.API/Properties/launchSettings.json` or open **Project Properties** ? **Debug** and update the launch profile `applicationUrl`.
3. Press F5 or use **Debug** ? **Start Debugging**.

## Run (dotnet CLI)
From the project folder:

```
dotnet run --project ShopSmartMCP.API
```

To override listening URLs at runtime (containers, CI, or local):

PowerShell:
```
$env:ASPNETCORE_URLS = "http://localhost:5002;https://localhost:5003"
dotnet run --project ShopSmartMCP.API
```

Linux/macOS:
```
ASPNETCORE_URLS="http://0.0.0.0:5002;https://0.0.0.0:5003" dotnet run --project ShopSmartMCP.API
```

Programmatic option (affects all runs): add this to `Program.cs` before `builder.Build()`:

```csharp
// example: listen on HTTP5002 and HTTPS5003
builder.WebHost.UseUrls("http://localhost:5002", "https://localhost:5003");
```

Notes:
- `launchSettings.json` affects only development launches from Visual Studio or `dotnet run` when using that profile.
- `ASPNETCORE_URLS` and `UseUrls` affect runtime behavior regardless of IDE.

## Endpoints
Base path: `{baseUrl}/api/mcp`

- POST `/search_product`
 - Body: `{ "query": "shirt", "maxPrice":50.0 }`
 - Response: `{ "products": [ ... ] }`

- POST `/place_order`
 - Body:
 ```json
 {
 "productId": "prod-1",
 "quantity":2,
 "address": "123 Main St",
 "paymentMethod": "card"
 }
 ```
 - Response: `OrderConfirmation` object (also appended to `mockdata/orders.json`).

- GET `/get_orders`
 - Returns saved orders from `mockdata/orders.json`.

- GET `/schema`
 - Returns the MCP schema file from `schema/mcp-schema.json`.

## Data files
- Seed or edit `mockdata/products.json` for available products.
- Orders are appended to `mockdata/orders.json` when `POST /place_order` is called.

## Tips
- If you want to silence C# nullability warnings for model classes, consider marking properties as `required` (C#11+) or make them nullable.
- For container deployments, bind to `0.0.0.0` and publish the app (`dotnet publish`).

## Contributing
Open a PR describing changes. Keep controller logic simple and stateless; persist only to `mockdata` for this demo.