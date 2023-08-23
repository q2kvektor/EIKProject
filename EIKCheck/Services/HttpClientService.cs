using System.Net;

namespace EIKCheck.Services;

public interface IHttpClientService
{
    string GetHTML(string url);
    byte[] GetBytes(string url);
}

public class HttpClientService : IHttpClientService
{
    static HttpClient client = new HttpClient();

    public string GetHTML(string url)
    {       
        Task<string> t = HttpGetResponse(url);  
        t.Wait();

        string response = t.Result;
        
        return response;
    }

    public byte[] GetBytes(string url)
    {
        Task<byte[]> t = HttpGetByteResponse(url);
        t.Wait();

        byte[] response = t.Result;

        return response;
    }
    
    static async Task<byte[]> HttpGetByteResponse(string url)
    {
        byte[] responseData = await client.GetByteArrayAsync(url);
        return responseData;
    }

    static async Task<string> HttpGetResponse(string url)
    {
        string responseData;

        using (var client = new HttpClient())
        {            
            using (var response = await client.GetAsync(url))
            {
                responseData = await response.Content.ReadAsStringAsync();

            }
        }

        return responseData;
    }
}
