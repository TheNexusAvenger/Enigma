using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Enigma.Core.Shim.Http;

namespace Enigma.Core.Test.TestShim;

public class TestHttpClient : IHttpClient
{
    /// <summary>
    /// Responses for HTTP request URLs.
    /// </summary>
    private readonly Dictionary<string, (HttpStatusCode, string)> _responses = new Dictionary<string, (HttpStatusCode, string)>(); 
    
    /// <summary>
    /// Sets the user agent for the HTTP client to use.
    /// </summary>
    /// <param name="userAgent">User agent to use.</param>
    public void SetUserAgent(string userAgent)
    {
        // No implementation.
    }

    /// <summary>
    /// Performs an HTTP GET request.
    /// </summary>
    /// <param name="url">URL for the request.</param>
    /// <returns>Response for the request.</returns>
    public Task<HttpResponseMessage> GetAsync(string url)
    {
        // Throw an exception if there is no stored URL.
        if (!this._responses.TryGetValue(url, out var response))
        {
            throw new IOException($"No response stored for {url}");
        }
        
        // Return the response.
        var (statusCode, body) = response;
        return Task.FromResult(new HttpResponseMessage()
        {
            StatusCode = statusCode,
            Content = new StringContent(body),
        });
    }

    /// <summary>
    /// Sets the response for an HTTP request.
    /// </summary>
    /// <param name="url">URL to match the response for.</param>
    /// <param name="statusCode">Status code of the response.</param>
    /// <param name="body">Body of the response.</param>
    public void SetResponse(string url, HttpStatusCode statusCode, string body)
    {
        this._responses[url] = (statusCode, body);
    }
}