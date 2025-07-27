using System.Text;
using System.Text.Json;

namespace Tau_CoinDesk_Api.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        private string PrettyJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return "";

            try
            {
                using var doc = JsonDocument.Parse(json);
                return JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
            }
            catch
            {
                return json;
            }
        }

        public async Task Invoke(HttpContext context)
        {
            context.Request.EnableBuffering();

            string requestBody = "";
            if (context.Request.ContentLength > 0 && context.Request.Body.CanRead)
            {
                context.Request.Body.Position = 0;
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            _logger.LogInformation($"----- HTTP REQUEST -----\n{context.Request.Method} {context.Request.Path}\nBody:\n{PrettyJson(requestBody)}");

            var originalBodyStream = context.Response.Body;
            await using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);
            
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            string responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            _logger.LogInformation($"----- HTTP RESPONSE -----\nStatus Code: {context.Response.StatusCode}\nBody:\n{PrettyJson(responseBodyText)}");

            await responseBody.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;
        }
    }
}