using NLua;

namespace AutomateEngine.NativeClasses;

public class Http
{
    private Lua lua;
    public Http(Lua lua) => this.lua = lua;

    public HttpInstance create() => new HttpInstance(lua);
}

public class HttpInstance
{
    private Lua lua;

    public HttpInstance(Lua lua) => this.lua = lua;

    public bool autosave_cookies = false;
    public bool auto_redirect = false;

    private HttpClientHandler _defaultHandler = new()
    {
        UseCookies = true,
        AllowAutoRedirect = false,
    };

    private void SetHandlerConfig()
    {
        _defaultHandler.UseCookies = autosave_cookies;
        _defaultHandler.AllowAutoRedirect = auto_redirect;
    }
    
    /// <summary>
    /// Send HTTP request to specified uri.
    /// </summary>
    /// <param name="uri">Send target uri</param>
    /// <param name="method">HTTP method (e.g. GET)</param>
    /// <param name="content">Request body (can be null)</param>
    /// <param name="headers">Request headers</param>
    /// <returns></returns>
    public Dictionary<object, object> send_request(string uri, string method, string content, LuaTable headers)
    {
        SetHandlerConfig();
        
        using var req = new HttpClient(_defaultHandler);
        
        // Setup the request message from arguments
        var message = new HttpRequestMessage();
        message.RequestUri = new Uri(uri);
        message.Method = method.ToUpper() switch
        {
            "GET" => HttpMethod.Get,
            "PUT" => HttpMethod.Put,
            "POST" => HttpMethod.Post,
            "DELETE" => HttpMethod.Delete,
            "HEAD" => HttpMethod.Head,
            "OPTIONS" => HttpMethod.Options,
            "TRACE" => HttpMethod.Trace,
            "PATCH" => HttpMethod.Patch,
            _ => new HttpMethod(method),
        };
        if (!string.IsNullOrEmpty(content)) message.Content = new StringContent(content);
        foreach (var key in headers.Keys)
        {
            var keyString = key.ToString();
            if (string.IsNullOrEmpty(keyString)) continue;
            message.Headers.Add(keyString, headers[keyString].ToString());
        }

        //TODO: add result value
        var result = new Dictionary<object, object>();
        try
        {
            // Send request
            var sendResult = req.Send(message);
            result.Add("has_syserror", false);
            result.Add("status_code", (int)sendResult.StatusCode);
            result.Add("content", sendResult.Content.ToString());
        }
        catch (HttpRequestException hrp)
        {
            result.Add("has_syserror", true);
            result.Add("error", hrp.Message);
        }

        return result;
    }

    public Dictionary<object, object> send_request(string uri, string method, LuaTable headers) =>
        send_request(uri, method, string.Empty, headers);
}