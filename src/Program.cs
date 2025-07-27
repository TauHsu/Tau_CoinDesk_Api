using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Tau_CoinDesk_Api.Data;
using Tau_CoinDesk_Api.Repositories;
using Tau_CoinDesk_Api.Services;
using Tau_CoinDesk_Api.Services.Encryption;
using Tau_CoinDesk_Api.Services.Security;
using Tau_CoinDesk_Api.Interfaces.Repositories;
using Tau_CoinDesk_Api.Interfaces.Services;
using Tau_CoinDesk_Api.Interfaces.Encryption;
using Tau_CoinDesk_Api.Interfaces.Security;
using Tau_CoinDesk_Api.Middlewares;

using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// 讀取 .env（僅本地開發用）
var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");
if (File.Exists(envPath))
{
    Env.Load(envPath);
}

builder.Configuration.AddEnvironmentVariables();

// 設定資料庫
var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// 其他設定
var aesKey = builder.Configuration["AES:Key"];
var aesIv = builder.Configuration["AES:IV"];
var rsaPrivate = builder.Configuration["RSA:PrivateKeyPath"];
var rsaPublic = builder.Configuration["RSA:PublicKeyPath"];

builder.Services.AddHttpClient<ICoinDeskService, CoinDeskService>();
builder.Services.AddScoped<IRatesService, RatesService>();
builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<ISecureCurrencyService, SecureCurrencyService>();

// AES, RSA
builder.Services.AddSingleton<IAesEncryptionStrategy, AesEncryptionStrategy>();
builder.Services.AddSingleton<IRsaCertificateStrategy, RsaCertificateService>();

// 多語系服務
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllers()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 語系設定
var supportedCultures = new[]
{
    new CultureInfo("en"),
    new CultureInfo("zh-TW"),
};

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"), // 預設語系為英文
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();

//app.UseHttpsRedirection();
app.MapControllers(); // 啟用 Controller 路由
app.Run();