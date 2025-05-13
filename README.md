# PostgreSQL 學習專案

這是一個記錄我學習 PostgreSQL 和 pgAdmin 的專案，包含 Render 上的雲端資料庫操作練習。

## 專案目標
- 學習 PostgreSQL 的 CRUD 操作（與 MS SQL 比較）。
- 使用 pgAdmin 連接到 Render 的 PostgreSQL 資料庫。
- 探索 JSONB 功能，結合 MongoDB 經驗。

## 環境設置
- **資料庫**：PostgreSQL（託管於 Render）
- **工具**：pgAdmin 4（繁體中文介面）
- **Render 資料庫 URL**：`postgresql://myuser:***@myhost.oregon-postgres.render.com/mydb`

## 範例語法
創建一個帶自增欄位的表格：
```sql
CREATE TABLE public.pra (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100)
);

INSERT INTO public.pra (name) VALUES ('Alice');
SELECT * FROM public.pra;
