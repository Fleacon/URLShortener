using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;

namespace URLShortenerCore;

public class WebServer
{
    public async void Start()
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8080/");
        listener.Start();
        Console.WriteLine("Server started and now Listening...");

        while (true)
        {
            HttpListenerContext context = await listener.GetContextAsync();
            HandleRequest(context);
        }
    }

    private void HandleRequest(HttpListenerContext context)
    {
        var request = context.Request;
        var response = context.Response;
        Console.WriteLine($"Received request: {request.HttpMethod} {request.RawUrl}");
        
        switch (request.HttpMethod)
        {
            case "GET":
                HandleGetRequest(request, response);
                break;
            case "POST":
                HandlePostRequest(request, response);
                break;
            default:
                response.StatusCode = 405;
                byte[] buffer = Encoding.UTF8.GetBytes("Method not allowed");
                response.ContentLength64 = buffer.Length;
                response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                break;
        }
    }

    private void HandlePostRequest(HttpListenerRequest request, HttpListenerResponse response)
    {
        Console.WriteLine($"Content-Type: {request.ContentType}");
        if (request.ContentType != "text/plain; charset=utf-8")
        {
            Console.WriteLine("Error");
            response.StatusCode = 406;
            return;
        }

        StreamReader reader = new(request.InputStream);
        var content = reader.ReadToEnd();
        Console.WriteLine($"Received: {content}");
        
        var shortUrl = AddShortUrl(content);
        byte[] buffer = Encoding.UTF8.GetBytes(shortUrl);
        
        response.ContentType = "text/plain";
        response.ContentLength64 = shortUrl.Length;
        response.StatusCode = 200;
        
        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.Close();
    }

    private void HandleGetRequest(HttpListenerRequest request, HttpListenerResponse response)
    {
        var path = request.Url.AbsolutePath;

        if (path.StartsWith("//database/GetAllURL"))
        {
            var urls = Database.GetInstance().GetAllUrls();
            byte[] buffer  = Encoding.UTF8.GetBytes(urls);
            
            response.ContentType = "text/plain; charset=utf-8";
            response.ContentLength64 = buffer.Length;
            response.StatusCode = 200;
            
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.Close();
        }
        else if (path.Length > 0 && path[0] == '/')
        {
            RedirectClient(request, response);
        }
    }

    private void RedirectClient(HttpListenerRequest request, HttpListenerResponse response)
    {
        Console.WriteLine($"Incoming URL: {request.Url}");
        var redirectUrl = GetRedirectUrl(request.RawUrl);
        Console.WriteLine(redirectUrl);
            
        if (redirectUrl is null)
        {
            Console.WriteLine("Invalid");
            response.StatusCode = 404;
        }
        else
        {
            Console.WriteLine($"Redirecting to: {redirectUrl}");
            response.StatusCode = 302;
            response.Redirect(redirectUrl);
            response.Close();
        }
    }
    
    private String? GetRedirectUrl(string url)
    {
        string shortUrl = url.Substring(url.LastIndexOf("/") + 1);
        return Database.GetInstance().SearchFullString(shortUrl);
    }

    private string AddShortUrl(string url)
    {
        if(!Regex.IsMatch(url, @"^(http|https)://"))
            url = "http://" + url;
        var rng = new RNGString();
        string shortenedURL = rng.generateRandomString();
        Database.GetInstance().Insert(shortenedURL,url);
        return shortenedURL;
    }
}