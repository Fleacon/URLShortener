// See https://aka.ms/new-console-template for more information

using System.Text.RegularExpressions;
using URLShortenerCore;
using System.Net.Http;
using System.Text;

var done = false;
var client = new HttpClient();
var baseadress = new Uri("http://localhost:8080");

Console.WriteLine("URL Shortener");
Console.WriteLine("----");

while (!done)
{
    Console.WriteLine("What would you like to do?");
    Console.WriteLine("1 - Shorten URL\n" +
                      "2 - View URLs\n" +
                      "0 - Exit");
    switch (int.Parse(Console.ReadLine()))
    {
        case 1: ShortenUrl();
            break;
        case 2: ViewUrl();
            break;
        case 0: done = true;
            break;
    }
}

void ShortenUrl()
{
    Console.Clear();
    Console.WriteLine("----Shorten URL----");
    Console.WriteLine("Enter URL:");
    string url;
    do
    {
         url = Console.ReadLine();
         if (Regex.IsMatch(url, @"[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)"))
         {
             PostUrlToServer(url);
             break;
         }
         else
         {
             Console.WriteLine("Invalid URL");
         }
    } while (true);
}

async void PostUrlToServer(string url)
{
    try
    {
        var content = new StringContent(url, Encoding.UTF8, @"text/plain");

        var response = await client.PostAsync(baseadress, content);
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Your shortened URL is: {baseadress}{responseString}");
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        throw;
    }
}

async void ViewUrl()
{
    Console.Clear();
    try
    {
        var response = await client.GetAsync($"{baseadress}/database/GetAllURL");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Console.WriteLine("----Viewing URLs----");
        Console.WriteLine(responseString);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        throw;
    }
}