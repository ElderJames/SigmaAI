using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Sigma.Core.Utils
{
    public class OpenAIHttpClientHandler(string endPoint, ILogger<OpenAIHttpClientHandler> logger) : HttpClientHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" && request.Content != null)
            {
                string requestBody = await request.Content.ReadAsStringAsync();
                //便于调试查看请求prompt
                logger.LogInformation("Request Url:@url RequestBody: @requestBody", new { url = request.RequestUri, requestBody });
            }

            request.RequestUri = new Uri(new Uri(endPoint), request.RequestUri?.PathAndQuery);

            // 接着，调用基类的 SendAsync 方法将你的修改后的请求发出去
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" && request.Content != null)
            {
                string requestBody = await response.Content.ReadAsStringAsync();
                //便于调试查看请求prompt
                logger.LogInformation(requestBody);
            }

            return response;
        }
    }

    public class OllamaHttpClientHandler(string endPoint, ILogger<OllamaHttpClientHandler> logger) : HttpClientHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" && request.Content != null)
            {
                string requestBody = await request.Content.ReadAsStringAsync();
                //便于调试查看请求prompt
                logger.LogInformation("Request Url:@url RequestBody: @requestBody", new { url = request.RequestUri, requestBody });
            }

            request.RequestUri = new Uri(new Uri(endPoint), request.RequestUri?.PathAndQuery.Replace("/v1/", "/api/"));

            // 接着，调用基类的 SendAsync 方法将你的修改后的请求发出去
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" && request.Content != null)
            {
                string requestBody = await response.Content.ReadAsStringAsync();
                //便于调试查看请求prompt
                logger.LogInformation(requestBody);
            }

            return response;
        }
    }
}