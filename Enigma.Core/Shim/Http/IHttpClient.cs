using System.Net.Http;
using System.Threading.Tasks;

namespace Enigma.Core.Shim.Http;

public interface IHttpClient
{
    /// <summary>
    /// Sets the user agent for the HTTP client to use.
    /// </summary>
    /// <param name="userAgent">User agent to use.</param>
    public void SetUserAgent(string userAgent);
    
    /// <summary>
    /// Performs an HTTP GET request.
    /// </summary>
    /// <param name="url">URL for the request.</param>
    /// <returns>Response for the request.</returns>
    public Task<HttpResponseMessage> GetAsync(string url);
}