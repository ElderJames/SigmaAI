using RestSharp;

namespace Sigma.Core.Domain.Interface
{
    public interface IHttpService
    {
        Task<RestResponse> PostAsync(string url, Object jsonBody);
    }
}
