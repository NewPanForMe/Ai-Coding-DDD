using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Client.Infrastructure.Middleware;

/// <summary>
/// 全局异常处理中间件，将未捕获的异常转换为结构化 HTTP 响应。
/// </summary>
public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    /// <summary>
    /// 初始化全局异常处理中间件。
    /// </summary>
    /// <param name="next">下一个中间件委托。</param>
    /// <param name="logger">日志记录器。</param>
    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// 处理 HTTP 请求，捕获并处理异常。
    /// </summary>
    /// <param name="context">当前 HTTP 上下文。</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "业务规则异常: {Path}", context.Request.Path);
            await WriteProblemDetailsAsync(context, ex, HttpStatusCode.BadRequest);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "参数校验异常: {Path}", context.Request.Path);
            await WriteProblemDetailsAsync(context, ex, HttpStatusCode.BadRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "未处理异常: {Path}", context.Request.Path);
            await WriteProblemDetailsAsync(context, ex, HttpStatusCode.InternalServerError);
        }
    }

    /// <summary>
    /// 写入 ProblemDetails 响应。
    /// </summary>
    /// <param name="context">当前 HTTP 上下文。</param>
    /// <param name="exception">捕获的异常。</param>
    /// <param name="statusCode">HTTP 状态码。</param>
    private static async Task WriteProblemDetailsAsync(
        HttpContext context, Exception exception, HttpStatusCode statusCode)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

        var response = new
        {
            type = $"https://httpstatuses.com/{(int)statusCode}",
            title = statusCode == HttpStatusCode.InternalServerError ? "服务器内部错误" : "请求无效",
            status = (int)statusCode,
            detail = isDevelopment ? exception.Message : GetPublicMessage(statusCode),
            instance = context.Request.Path
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }

    /// <summary>
    /// 获取面向客户端的错误消息。
    /// </summary>
    /// <param name="statusCode">HTTP 状态码。</param>
    /// <returns>错误消息文本。</returns>
    private static string GetPublicMessage(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.BadRequest => "请求参数无效或违反业务规则，请检查输入。",
            _ => "服务器发生内部错误，请稍后重试。"
        };
    }
}
