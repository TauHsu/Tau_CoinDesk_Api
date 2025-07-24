# 使用 .NET SDK 來建置專案
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 複製 csproj 並還原套件
COPY src/Tau_CoinDesk_Api.csproj ./ 
RUN dotnet restore Tau_CoinDesk_Api.csproj

# 複製專案所有檔案並建置
COPY src/. .
RUN dotnet publish -c Release -o /app

# 使用較小的 runtime 映像檔
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

# 開放 8080 埠口
EXPOSE 8080

# 容器啟動時執行 API 並指定監聽 8080
ENTRYPOINT ["dotnet", "Tau_CoinDesk_Api.dll", "--urls", "http://0.0.0.0:8080"]