using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace Ambev.DeveloperEvaluation.Tests.Shared;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    
    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<HttpResponseMessage> PostAsync(string route, object payload, string? accessToken = null)
    {
        var requestContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        
        return await _httpClient.PostAsync(route, requestContent);
    }
    
    public async Task<HttpResponseMessage> GetAsync(string route, string? accessToken = null)
    {
        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        
        return await _httpClient.GetAsync(route);
    }
}