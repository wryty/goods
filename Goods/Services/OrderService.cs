using GoodsApi.Data.Entities;


namespace Goods.Services;

public class OrderService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly SessionService _sessionService;
    private string _apiBaseUrl;
    public OrderService(IHttpClientFactory clientFactory, SessionService sessionService)
    {
        _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        _apiBaseUrl = "https://localhost:7210";
    }

    private async Task<HttpClient> GetClientAsync()
    {
        var client = _clientFactory.CreateClient();
        var token = await _sessionService.GetTokenAsync();

        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }

    public async Task<Order> GetOrderAsync(long id)
    {
        var client = await GetClientAsync();

        var response = await client.GetFromJsonAsync<Order>($"{_apiBaseUrl}/api/Order/{id}");

        return response;
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync()
    {
        var client = await GetClientAsync();

        var response = await client.GetFromJsonAsync<IEnumerable<Order>>($"{_apiBaseUrl}/api/Order");

        return response ?? Array.Empty<Order>();
    }

    public async Task<IEnumerable<OrderItem>> GetOrderItemsAsync(long id)
    {
        var client = await GetClientAsync();

        var response = await client.GetFromJsonAsync<IEnumerable<OrderItem>>($"{_apiBaseUrl}/items/{id}");

        return response ?? Array.Empty<OrderItem>();
    }

    public async Task<long> CreateOrderAsync(List<long> itemsId)
    {
        var client = await GetClientAsync();
        var response = await client.PostAsJsonAsync($"{_apiBaseUrl}/api/Order", itemsId);

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadFromJsonAsync<OrderCreationResponse>();
            return responseBody.OrderId;
        }

        return 0;
    }

    public async Task<bool> DeleteOrderAsync(long id)
    {
        var client = await GetClientAsync();

        var response = await client.DeleteAsync($"{_apiBaseUrl}/api/Order/{id}");

        return response.IsSuccessStatusCode;
    }
    public class OrderCreationResponse
    {
        public long OrderId { get; set; }
        public string Message { get; set; }
    }
}
