using System.Net.Http;
using System.Threading.Tasks;

namespace Enigma.Core.Shim.Http;

public class HttpClientWrapper : IHttpClient
{
    /// <summary>
    /// Http client used to send requests.
    /// </summary>
    private readonly HttpClient _httpClient = new HttpClient();
    
    /// <summary>
    /// Sets the user agent for the HTTP client to use.
    /// </summary>
    /// <param name="userAgent">User agent to use.</param>
    public void SetUserAgent(string userAgent)
    {
        this._httpClient.DefaultRequestHeaders.Add("User-Agent", userAgent);
    }

    /// <summary>
    /// Performs an HTTP GET request.
    /// </summary>
    /// <param name="url">URL for the request.</param>
    /// <returns>Response for the request.</returns>
    public async Task<HttpResponseMessage> GetAsync(string url)
    {
        return await this._httpClient.GetAsync(url);
    }
}