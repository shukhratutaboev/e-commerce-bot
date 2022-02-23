using System.ComponentModel;
using System.Text.Json;
using bot.DTO;

namespace bot.Services;
public class DashboardClient
{
    private readonly HttpClient _client;
    private readonly ILogger<DashboardClient> _logger;
    private readonly HttpClient client1;

    public DashboardClient(HttpClient client, ILogger<DashboardClient> logger)
    {
        _client = client;
        _logger = logger;
        HttpClientHandler clientHandler = new HttpClientHandler();
        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

        // Pass the handler to httpclient(from you are calling api)
        client1 = new HttpClient(clientHandler);
        client1.BaseAddress = new Uri("https://localhost:7070/api/");
    }
    public async Task<(List<Category> categories, bool isSuccess, Exception exception)> GetCategoriesAsync()
    {
        var query = "category/";
        // using var httpResponse = await _client.GetAsync(query);
        using var httpResponse = await client1.GetAsync(query);
        if(httpResponse.IsSuccessStatusCode)
        {
            var json = await httpResponse.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<List<Category>>(json);
            return (data, true, null);
        }
        return (null, false, new Exception(httpResponse.ReasonPhrase));
    }
    public async Task<(List<Item> items, bool isSuccess, Exception exception)> GetItemsByCategoryAsync(string category)
    {
        var query = "item/";
        using var httpResponse = await client1.GetAsync(query);
        if(httpResponse.IsSuccessStatusCode)
        {
            var json = await httpResponse.Content.ReadAsStringAsync();
            if(category == "all") return (JsonSerializer.Deserialize<List<Item>>(json), true, null);
            var data = JsonSerializer.Deserialize<List<Item>>(json).Where(i => i.Category.Name == category).ToList();
            return (data, true, null);
        }
        return (null, false, new Exception(httpResponse.ReasonPhrase));
    }
    public async  Task<(Item item, bool isSuccess, Exception exception)> GetItemAsync(string id)
    {
        var query = $"item/{id}";
        using var httpResponse = await client1.GetAsync(query);
        if(httpResponse.IsSuccessStatusCode)
        {
            var json = await httpResponse.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<Item>(json);
            return (data, true, null);
        }
        return (null, false, new Exception(httpResponse.ReasonPhrase));
    }
}