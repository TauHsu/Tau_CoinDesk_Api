# 徐韜 ASP.NET Core 8.0 Coindesk API 專案
## 專案簡介
此專案使用 ASP.NET Core 8.0 Web API，整合 Coindesk API，並實作：
1. 幣別資料庫維護功能 (查詢、新增、修改、刪除）。
   - 取得幣別列表、取得單一幣別，新增、修改、刪除幣別，共 5 支 API。
   - 最下方附上 建立 SQL 語法。
2. 呼叫 Coindesk API，轉換並輸出新的 API。
   - 提供匯率與更新時間，整合幣別中文名稱。
3. 使用 Entity Framework Core，資料庫以 Docker 部署之 SQL Server (非 LocalDB)。
4. 所有功能包含單元測試。
5. 支援 Docker 部署。
## [Demo Video](https://youtu.be/elVZqB_dsi0?si=S4z5fS4BEnFLk7MV)
[![Demo Video](http://img.youtube.com/vi/elVZqB_dsi0/default.jpg)](https://www.youtube.com/watch?v=elVZqB_dsi0)

---

## 環境需求
- .NET 8.0 SDK (開發與執行 API)
- Docker & Docker Compose (用來啟動 SQL Server 與 API 容器)
- Dbeaver (選用，用於檢視資料庫內容)
本專案使用 Docker 部署的 SQL Server 2022 (非 LocalDB)，因此可跨 Windows / Mac / Linux 環境運行。

---

## 加分功能 (本專案已實作)
- [x] API Request & Response Log (於 Demo 影片展示)
- [x] Error Handling (於 Demo 影片展示)
- [x] Swagger UI (/swagger)
- [x] 多語系設計 (支援中英文，於 Demo 影片展示)
- [x] Design Pattern (於下方 **專案架構** 展示說明)
- [x] Docker 部署
- [x] AES 加解密（幣別代碼欄位加密存儲）
- [x] RSA 加解密（匯率資料簽章、驗證）

---

## 專案架構
```bash
Tau_CoinDesk_Api (ASP .NET Core 8.0)/
├── Migrations
├── Properties/
│   ├── launchSettings.json
├── src/
│   ├── Controllers/                            # API 控制器 (負責接收 HTTP 請求)
│   │   ├── CurrenciesController.cs             # 幣別 CRUD API
│   │   ├── RatesController.cs                  # Coindesk 匯率 API 轉換輸出
│   │   ├── SecureCurrenciesController.cs       # 加密幣別 API (AES 加解密)
│   ├── Data/
│   │   ├── AppDbContext.cs                     # EF Core 資料庫上下文 (DB 連線與 DbSet)
│   ├── Exceptions/
│   │   ├── AppException.cs                     # 自訂例外處理 (統一回應錯誤)
│   ├── Interfaces/                             # 介面 (依賴反轉，便於測試與維護)
│   │   ├── Encryption/
│   │   │   ├── IAesEncryptionStrategy.cs       # AES 加解密策略介面
│   │   ├── Repositories/
│   │   │   ├── ICurrencyRepository.cs          # 幣別資料庫存取介面
│   │   ├── Security/
│   │   │   ├── IRsaCertificateStrategy.cs      # RSA 憑證加解密策略介面
│   │   ├── Services/
│   │   │   ├── ICoinDeskService.cs             # Coindesk API 服務介面
│   │   │   ├── ICurrencySerivce.cs             # 幣別業務邏輯介面
│   │   │   ├── IRatesService.cs                # 匯率處理邏輯介面
│   │   │   ├── ISecureCurrencyService.cs       # 加密幣別業務邏輯介面
│   ├── Middlewares/                            # 中介層 (全域攔截功能)
│   │   ├── ErrorHandlingMiddleware.cs          # 全域錯誤攔截與統一回應格式
│   │   ├── LoggingMiddleware.cs                # 請求與回應記錄 (Request/Response Log)
│   ├── Models/
│   │   ├── Dto/                                # 資料傳輸物件 (API 專用結構)
│   │   │   ├── ApiResponse.cs                  # 統一 API 回應格式
│   │   │   ├── CurrencyDto.cs                  # 幣別資料傳輸物件
│   │   │   ├── RatesResponseDto.cs             # 匯率 API 回應物件
│   │   │   ├── VerifyRatesRequestDto.cs        # 匯率驗證請求物件
│   │   ├── Entities/
│   │   │   ├── Currency.cs                     # 幣別資料庫實體
│   ├── Repositories/
│   │   ├── CurrencyRepository.cs               # 幣別資料存取實作 (EF Core)
│   ├── Resources/                              # 多語系資源檔
│   │   ├── SharedResource.en.resx              # 英文多語系字串
│   │   ├── SharedResource.zh-TW.resx           # 繁體中文多語系字串
│   ├── Services/                               # 業務邏輯與服務層
│   │   ├── Encryption/
│   │   │   ├── AesEncryptionStrategy.cs        # AES 加密/解密實作
│   │   ├── Security/
│   │   │   ├── RsaCertificateStrategy.cs       # RSA 憑證加解密實作
│   │   ├── CoinDeskService.cs                  # 取得並轉換 Coindesk API 資料
│   │   ├── CurrencySerivce.cs                  # 幣別業務邏輯實作
│   │   ├── RatesService.cs                     # 匯率 API 資料轉換與處理
│   │   ├── SecureCurrencyService.cs            # 加密幣別邏輯 (AES/RSA 支援)
│   ├── appsettings.json                        # 專案主要設定檔 (資料庫連線、加解密金鑰、應用程式設定)
│   ├── Program.cs                              # 專案入口 (DI 註冊、中介層設定)
│   ├── SharedResource.cs                       # 多語系 Resource 對應入口
│   ├── Tau_CoinDesk_Api.csproj                 # 專案檔案 (NuGet 套件與建置設定)
├── tests/Tau_CoinDesk_Api.Tests                # 單元測試專案
│   ├── Controllers/
│   │   ├── CurrenciesControllerTests.cs        # 測試幣別 API
│   │   ├── RatesControllerTests.cs             # 測試匯率 API
│   │   ├── SecureCurrenciesControllerTests.cs  # 測試加密幣別 API
│   ├── Services/
│   │   ├── CoinDeskServiceTests.cs             # 測試 Coindesk 資料處理
│   │   ├── CurrencySerivceTests.cs             # 測試幣別邏輯
│   │   ├── RatesServiceTests.cs                # 測試匯率邏輯
│   │   ├── SecureCurrencyServiceTests.cs       # 測試加密邏輯
│   ├── Tau_CoinDesk_Api.Tests.csproj           # 測試專案設定
├── .env.example                                # 環境變數範例 (資料庫連線、加密 Key)
├── .gitignore                                  # Git 忽略設定
├── appsettings.Development.json                # 開發環境設定檔 (DB、Log 等)
├── docker-compose.yml                          # Docker Compose 設定 (DB + API)
├── Dockerfile                                  # API 容器化設定
├── README.md                                   # 專案說明文件
├── Tau_CoinDesk_Api.http                       # API 測試請求檔 (VS Code/HTTP Client)
├── Tau_CoinDesk_Api.sln                        # Visual Studio 解決方案檔
```

---

## 安裝
以下將會引導你如何安裝此專案到你的電腦上。
#### 下載專案
```bash
git clone https://github.com/your-username/Tau_CoinDesk_Api.git
```
#### 移動到專案內
```bash
cd Tau_CoinDesk_Api
```
#### 環境變數設定
複製 .env.example 為 .env，並依需求調整內容（尤其是 AES/RSA 設定與 DB 密碼）：
```bash
cp .env.example .env
```

---

#### 一鍵啟動專案 (Docker 全部運行)
專案內已包含 **API 與 SQL Server** 的 `docker-compose.yml`，可一鍵啟動環境。
###### 步驟 1. 啟動服務 (API + SQL Server)
```bash
docker-compose up -d
```
此指令會啟動：
- db：SQL Server (帳號：sa / 密碼：Coindesk123)
- api：ASP.NET Core 8.0 API (http://localhost:8080
###### 步驟 2. 初始化資料庫 (EF Core Migration)
新增 Migration：
```bash
docker exec -it coindesk-api dotnet ef database update
```
執行 Migration：
```bash
dotnet ef database update --project src/Tau_CoinDesk_Api.csproj
```

###### 步驟 3. 測試 API
- API 入口：http://localhost:8080
- Swagger UI：http://localhost:8080/swagger

---

## 本地開發 (API 本地執行，資料庫用 Docker)
#### 啟動 SQL Server 容器
```bash
docker-compose up -d db
```
#### 編輯 .env，將資料庫連線改為：
```bash
DB_CONNECTION=Server=localhost,1433;Database=CoinDeskDb;User Id=sa;Password=Coindesk123;TrustServerCertificate=True;
```
#### 在本地端執行 Migration：
新增、執行 Migration(同上述 Docker 提到之方法)

#### 啟動 API（本地端）：
```bash
dotnet run --project src/Tau_CoinDesk_Api.csproj
```
#### 測試 API：
- API 入口：http://localhost:5000
- Swagger UI：http://localhost:5000/swagger
---

## 環境變數說明
```bash
#建議測試時，先將 .env.example 複製為 .env
# cp .env.example .env

# Database
DB_CONNECTION=Server=localhost,1433;Database=CoinDeskDb;User Id=sa;Password=Coindesk123;TrustServerCertificate=True;

# AES Encryption
# 注意：AES_KEY 長度需為 32 bytes，AES_IV 需為 16 bytes。
AES_KEY=your_AesKey_32bytes
AES_IV=your_AesIv_16bytes

# RSA Keys
# 只需提供路徑，不需手動提供私鑰
# 專案啟動時，如路徑下不存在檔案，會自動生成 2048-bit RSA 金鑰對至 Keys/ 資料夾
RSA_PRIVATE_KEY_PATH=Keys/private.xml
RSA_PUBLIC_KEY_PATH=Keys/public.xml
```

---

## 單元測試
本專案使用 xUnit 作為測試框架，主要覆蓋：
- Controllers：驗證 API 路由、回應格式與狀態碼
- Services：測試商業邏輯、資料轉換與錯誤處理
執行測試(於 Tau_CoinDesk_Api 資料夾)：
```bash
dotnet test
```

---

## SQL 語法
建立 Currencies 資料表
```sql
CREATE TABLE Currencies (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Code NVARCHAR(50) NOT NULL,
    ChineseName NVARCHAR(50) NOT NULL
);
```
建立唯一索引，避免重複幣別代碼
```sql
CREATE UNIQUE INDEX IX_Currencies_Code ON Currencies(Code);
```
新增 幣別
```sql
INSERT INTO Currencies (Id, Code, ChineseName)
VALUES (NEWID(),N'GBP', N'英鎊');
```
查詢 幣別
```sql
SELECT Id, Code, ChineseName
FROM Currencies
ORDER BY Code ASC;
```
修改 幣別
```sql
UPDATE Currencies
SET ChineseName = N'美金'
WHERE Code = N'USD';
(或使用 Id 查詢修改 Id = N'070447F0-9DCC-4080-A21A-35B20005F5B4';)
```
刪除 幣別
```bash
DELETE FROM Currencies
WHERE Code = N'USD';
(或使用 Id 查詢刪除 WHERE Id = N'070447F0-9DCC-4080-A21A-35B20005F5B4';)
```

---

## 加密技術應用簡介
本專案使用以下加密技術保障資料安全：
- AES 加解密：對敏感資料（如幣別代碼）進行對稱式加密，確保資料在儲存和傳輸過程中不被竊取。
- RSA 簽章與驗證：使用非對稱式加密技術，對匯率資料進行數位簽章，確保資料的完整性與真實性，並用公鑰進行驗證防止偽造。

---

## 關於作者
```bash
姓名: 徐韜 
Email: jason850629@gmail.com
GitHub: https://github.com/TauHsu
```
