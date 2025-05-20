// Controllers/ProductsController.cs
using ApiPraPostgreSQL.Models; // 引用 Models 命名空間下的類別，例如 Product
using ApiPraPostgreSQL.Services; // 引用 Services 命名空間下的介面或類別，例如 IProductService
using Microsoft.AspNetCore.Mvc; // 引用 ASP.NET Core MVC 相關的類別，例如 [ApiController], [Route], ControllerBase, ActionResult
using Microsoft.AspNetCore.Mvc.Routing;
using System.Collections.Generic; // 引用泛型集合介面，例如 IEnumerable
using System.Threading.Tasks; // 引用非同步操作相關的類別，例如 Task

namespace ApiPraPostgreSQL.Controllers // 定義控制器所在的命名空間
{
    [ApiController] // 標記此類別為 API 控制器，提供自動的 HTTP 響應行為
    [Route("api/[controller]")] // 定義此控制器的路由前綴，"[controller]" 會被替換為類別名稱 (Products)
    public class ProductsController : ControllerBase // 繼承 ControllerBase，提供基本的控制器功能
    {
        private readonly IProductService _productService; // 宣告一個唯讀的 IProductService 介面變數，用於依賴注入

        /// <summary>
        /// 建構子，用於接收 IProductService 的實例，實現依賴注入
        /// </summary>
        /// <param name="productService"></param>
        public ProductsController(IProductService productService)
        {
            _productService = productService; // 將傳入的 IProductService 實例賦值給 _productService 變數
        }

        // async 表示非同 函式內部會有非同操作 不必等 以免堵塞
        // Task 只要非同 都必須用 Task包外層
        // ActionResult 除了回傳值外 能額外給回應狀態
        // IEnumerable 只要枚舉類 都可用它 就不必指頂定 array list等

        /// <summary>
        /// HTTP GET 請求處理方法，用於取得所有產品
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HttpGet("pro")] // 相對於控制器的路由前綴
        [Route("products")]
        [Route("haha")]
        // 發送: https://localhost:7181/api/Products/haha
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync(); // 非同步呼叫 _productService 的 GetAllProductsAsync 方法，取得所有產品的列表
            return Ok(products); // 回傳 HTTP 200 OK 狀態碼，並將產品列表作為響應內容
        }

        /// <summary>
        /// HTTP GET 請求處理方法，用於根據 ID 取得特定產品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")] // "{id}" 是路由參數，用於接收產品的 ID
        // 發送: https://localhost:7181/api/Products/1
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id); // 非同步呼叫 _productService 的 GetProductByIdAsync 方法，根據 ID 取得特定產品
            if (product == null) // 檢查是否找到該產品
            {
                //Product productNull = new Product();
                //return NotFound(productNull); // 如果找不到產品，回傳 HTTP 404 Not Found 狀態碼
                return NotFound();
            }
            return Ok(product); // 如果找到產品，回傳 HTTP 200 OK 狀態碼，並將產品作為響應內容
            // 此回傳未整合
        }

        /// <summary>
        /// HTTP POST 請求處理方法，根據 ID 取得特定產品 (使用body發送)_直接給int
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("getProductFromBody_int")] // 自訂路由名稱，避免與原有的 GetProduct 衝突
        public async Task<ActionResult<Product>> GetProductFromBody([FromBody] int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        /// <summary>
        /// HTTP POST 請求處理方法，根據 ID 取得特定產品 (使用body發送)_直接給json
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("getProductFromBody_json")]
        public async Task<ActionResult<Product>> GetProductFromBody([FromBody] ProductIdRequest request)
        {
            if (request == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState); // 處理請求體為空或模型驗證失敗的情況
            }

            var productId = request.Id; // 從請求物件中獲取 ID
            var product = await _productService.GetProductByIdAsync(productId);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // HTTP POST 請求處理方法，用於創建新的產品
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            // 檢查模型狀態是否有效（例如，是否滿足資料驗證規則）
            if (ModelState.IsValid)
            {
                await _productService.CreateProductAsync(product); // 非同步呼叫 _productService 的 CreateProductAsync 方法，創建新的產品
                // 回傳 HTTP 201 Created 狀態碼，並在 Location 標頭中包含新創建產品的 URI
                return CreatedAtAction(nameof(GetProduct), new { id = product.id }, product);
            }
            // 如果模型狀態無效，回傳 HTTP 400 Bad Request 狀態碼，並包含驗證錯誤資訊
            return BadRequest(ModelState);
        }

        // HTTP PUT 請求處理方法，用於根據 ID 更新現有的產品
        [HttpPut("{id}")] // "{id}" 是路由參數，用於接收要更新的產品的 ID
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            // 檢查路由中的 ID 是否與請求體中的產品 ID 相符
            if (id != product.id)
            {
                return BadRequest(); // 如果 ID 不符，回傳 HTTP 400 Bad Request 狀態碼
            }

            // 檢查模型狀態是否有效
            if (ModelState.IsValid)
            {
                var updated = await _productService.UpdateProductAsync(id, product); // 非同步呼叫 _productService 的 UpdateProductAsync 方法，更新產品
                if (updated) // 檢查產品是否成功更新
                {
                    return NoContent(); // 如果更新成功，回傳 HTTP 204 No Content 狀態碼
                }
                return NotFound(); // 如果找不到要更新的產品，回傳 HTTP 404 Not Found 狀態碼
            }
            // 如果模型狀態無效，回傳 HTTP 400 Bad Request 狀態碼，並包含驗證錯誤資訊
            return BadRequest(ModelState);
        }

        // HTTP DELETE 請求處理方法，用於根據 ID 刪除現有的產品
        [HttpDelete("{id}")] // "{id}" 是路由參數，用於接收要刪除的產品的 ID
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var deleted = await _productService.DeleteProductAsync(id); // 非同步呼叫 _productService 的 DeleteProductAsync 方法，刪除產品
            if (deleted) // 檢查產品是否成功刪除
            {
                return NoContent(); // 如果刪除成功，回傳 HTTP 204 No Content 狀態碼
            }
            return NotFound(); // 如果找不到要刪除的產品，回傳 HTTP 404 Not Found 狀態碼
        }
    }
}