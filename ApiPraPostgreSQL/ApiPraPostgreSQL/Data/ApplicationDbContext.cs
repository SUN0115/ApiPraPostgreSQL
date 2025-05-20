// Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using ApiPraPostgreSQL.Models;

namespace ApiPraPostgreSQL.Data
{
    public class ApplicationDbContext : DbContext
    {
        // 建構函式，接收 DbContextOptions<ApplicationDbContext> 參數。
        // DbContextOptions 包含資料庫連線設定（例如連線字串、資料庫提供者）。
        // 這參數由依賴注入 (Dependency Injection) 提供，通常在 Program.cs 或 Startup.cs 中配置。
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            // 將 options 傳遞給基類 DbContext 的建構函式，完成連線配置。
            // 這裡沒有額外邏輯，因為 EF Core 會根據 options 自動處理連線。
        }

        // 定義 DbSet<Product> 屬性，名為 Products。
        // DbSet<T> 是 EF Core 用來表示資料庫中某個表的集合，T 是對應的實體類別（這裡是 Product）。
        // Products 屬性告訴 EF Core：Product 實體類別對應到資料庫中的一個表（預設表名為 "Products"）。
        // 透過這個屬性，你可以執行 CRUD 操作，例如 Products.Add()、Products.Find() 等。
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().ToTable("product");
            base.OnModelCreating(modelBuilder);
        }
    }
}