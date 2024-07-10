using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Enigma.Core.Web.GitHub.Model;

namespace Enigma.Core.Shim.Http;

public class HttpClientWrapper : IHttpClient
{
    /// <summary>
    /// Http client used to send requests.
    /// </summary>
    // TODO: Windows Defender is triggered by this.
    // private readonly HttpClient _httpClient = new HttpClient();
    
    /// <summary>
    /// Sets the user agent for the HTTP client to use.
    /// </summary>
    /// <param name="userAgent">User agent to use.</param>
    public void SetUserAgent(string userAgent)
    {
        // TODO: Windows Defender is triggered by this.
        // this._httpClient.DefaultRequestHeaders.Add("User-Agent", userAgent);
    }

    /// <summary>
    /// Performs an HTTP GET request.
    /// </summary>
    /// <param name="url">URL for the request.</param>
    /// <returns>Response for the request.</returns>
    public async Task<HttpResponseMessage> GetAsync(string url)
    {
        // TODO: Windows Defender is triggered by this.
        // return await this._httpClient.GetAsync(url);
        // To avoid other errors, this dummy code is used.
        // Currently, GetAsync is only used for the update check.
        var versionData = await ProjectVersionData.GetVersionDataAsync();
        return new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent($"[{{\"name\":\"{versionData?.Version}\"}}]"),
        };
    }
}