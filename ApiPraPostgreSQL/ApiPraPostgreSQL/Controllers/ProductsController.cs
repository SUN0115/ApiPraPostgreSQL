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
        [HttpPost("getProductFromBody_int")]
        // 發送: https://localhost:7181/api/Products/getProductFromBody_int
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
        // 發送: https://localhost:7181/api/Products/getProductFromBody_json
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

        /// <summary>
        /// HTTP POST 請求處理方法，用於創建新的產品
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        // 發送: 
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            // 檢查模型狀態是否有效（例如，是否滿足資料驗證規則，這些規則通常在 Product 模型類別中使用 Data Annotations 定義）
            if (ModelState.IsValid)
            {
                await _productService.CreateProductAsync(product); // 非同步呼叫 _productService 的 CreateProductAsync 方法，將傳入的 Product 物件保存到資料庫中
                // 回傳 HTTP 201 Created 狀態碼，表示資源已成功創建，並在 Location 標頭中包含新創建產品的 URI，方便客戶端獲取新資源
                return CreatedAtAction(nameof(GetProduct), new { id = product.id }, product);
            }
            // 如果模型狀態無效（例如，缺少必要的欄位或欄位格式不正確），回傳 HTTP 400 Bad Request 狀態碼，並包含 ModelState 中的驗證錯誤資訊，告知客戶端請求存在問題
            return BadRequest(ModelState);
        }

        /// <summary>
        /// HTTP PUT 請求處理方法，用於修改產品
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPut]
        // 發送(put): https://localhost:7181/api/Products
        public async Task<IActionResult> UpdateProduct(Product product)
        {
            // 檢查模型狀態是否有效
            if (ModelState.IsValid)
            {
                if (product == null || product.id == 0) // 確保請求體不為空且包含有效的 ID
                {
                    return BadRequest("請求體必須包含有效的產品資訊，包括 ID。");
                }

                // 可改回傳值
                var updated = await _productService.UpdateProductAsync(product); // 從 Product 物件中獲取 ID
                if (updated)
                {
                    return NoContent();
                }
                return NotFound(); // 如果找不到具有該 ID 的產品
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// HTTP DELETE 請求處理方法，用於根據 ID 刪除現有的產品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete] 
        // 發送(Delete): https://localhost:7181/api/Products
        public async Task<IActionResult> DeleteProduct([FromBody] ProductIdRequest request)
        {
            if (request == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState); // 處理請求體為空或模型驗證失敗的情況
            }
            var deleted = await _productService.DeleteProductAsync(request.Id); // 非同步呼叫 _productService 的 DeleteProductAsync 方法，刪除產品
            if (deleted) // 檢查產品是否成功刪除
            {
                return NoContent(); // 如果刪除成功，回傳 HTTP 204 No Content 狀態碼
            }
            return NotFound(); // 如果找不到要刪除的產品，回傳 HTTP 404 Not Found 狀態碼
        }
    }
}