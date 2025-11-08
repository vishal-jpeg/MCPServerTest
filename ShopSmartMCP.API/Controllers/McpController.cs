using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace McpDemo.Controllers
{
    [ApiController]
    [Route("api/mcp")]
    public class McpController : ControllerBase
    {
        private readonly string productsFile = Path.Combine(Directory.GetCurrentDirectory(), "mockdata", "products.json");
        private readonly string ordersFile = Path.Combine(Directory.GetCurrentDirectory(), "mockdata", "orders.json");

        // -------------------- SEARCH PRODUCT --------------------
        [HttpPost("search_product")]
        public IActionResult SearchProduct([FromBody] ProductSearchRequest request)
        {
            try
            {
                Console.WriteLine($"SearchProduct Request: {JsonConvert.SerializeObject(request)}");

                var jsonData = System.IO.File.ReadAllText(productsFile);
                var products = JsonConvert.DeserializeObject<List<Product>>(jsonData) ?? new List<Product>();

                var results = products
                    .Where(p => p.Name.Contains(request.Query, StringComparison.OrdinalIgnoreCase)
                             && p.Price <= request.MaxPrice)
                    .ToList();

                Console.WriteLine($"Found {results.Count} products");
                return Ok(new { products = results });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SearchProduct: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // -------------------- PLACE ORDER --------------------
        [HttpPost("place_order")]
        public IActionResult PlaceOrder([FromBody] OrderRequest order)
        {
            try
            {
                Console.WriteLine($"PlaceOrder Request: {JsonConvert.SerializeObject(order)}");

                if (order == null || string.IsNullOrEmpty(order.ProductId))
                    return BadRequest(new { message = "Invalid order request" });

                var jsonData = System.IO.File.ReadAllText(productsFile);
                var products = JsonConvert.DeserializeObject<List<Product>>(jsonData) ?? new List<Product>();
                var selectedProduct = products.FirstOrDefault(p => p.Id == order.ProductId);

                if (selectedProduct == null)
                    return NotFound(new { message = "Product not found" });

                decimal totalAmount = selectedProduct.Price * order.Quantity;

                var confirmation = new OrderConfirmation
                {
                    OrderId = Guid.NewGuid().ToString(),
                    ProductId = selectedProduct.Id,
                    ProductName = selectedProduct.Name,
                    Quantity = order.Quantity,
                    TotalAmount = totalAmount,
                    Status = "Confirmed",
                    EstimatedDelivery = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd")
                };

                // Save order in orders.json
                var existingOrders = System.IO.File.Exists(ordersFile)
                    ? JsonConvert.DeserializeObject<List<OrderConfirmation>>(System.IO.File.ReadAllText(ordersFile)) ?? new List<OrderConfirmation>()
                    : new List<OrderConfirmation>();

                existingOrders.Add(confirmation);
                System.IO.File.WriteAllText(ordersFile, JsonConvert.SerializeObject(existingOrders, Newtonsoft.Json.Formatting.Indented));

                Console.WriteLine($"Order Placed: {confirmation.OrderId}");

                return Ok(confirmation);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PlaceOrder: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        //-------------------- GET ORDERS --------------------
        [HttpGet("get_orders")]
        public IActionResult GetOrders()
        {
            if (!System.IO.File.Exists(ordersFile))
                return NotFound("Order data file not found.");

            try
            {
                var json = System.IO.File.ReadAllText(ordersFile);
                var orders = JsonConvert.DeserializeObject<List<OrderConfirmation>>(json) ?? new List<OrderConfirmation>();
                return Ok(new { orders });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetOrders: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // -------------------- MCP SCHEMA ------------------------
        [HttpGet("schema")]
        public IActionResult GetMcpSchema()
        {
            var schemaPath = Path.Combine(Directory.GetCurrentDirectory(),"schema" , "mcp-schema.json");
            if (!System.IO.File.Exists(schemaPath))
                return NotFound(new { message = "Schema file not found" });

            var schemaContent = System.IO.File.ReadAllText(schemaPath);
            return Content(schemaContent, "application/json");
        }

    }


    // -------------------- MODEL CLASSES --------------------
    public class Product
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }
    }

    public class ProductSearchRequest
    {
        [JsonProperty("query")]
        public string Query { get; set; }

        [JsonProperty("maxPrice")]
        public decimal MaxPrice { get; set; }
    }

    public class OrderRequest
    {
        [JsonProperty("productId")]
        public string ProductId { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("paymentMethod")]
        public string PaymentMethod { get; set; }
    }

    public class OrderConfirmation
    {
        [JsonProperty("orderId")]
        public string OrderId { get; set; }

        [JsonProperty("productId")]
        public string ProductId { get; set; }

        [JsonProperty("productName")]
        public string ProductName { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("totalAmount")]
        public decimal TotalAmount { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("estimatedDelivery")]
        public string EstimatedDelivery { get; set; }
    }
}
