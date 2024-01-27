using GoodsApi.Data.Entities;
using GoodsApi.Models;


namespace Goods.Services;

public class CartService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly SessionService _sessionService;
    private string _apiBaseUrl;
    public CartService(IHttpClientFactory clientFactory, SessionService sessionService)
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

    public async Task<IEnumerable<CartItem>> GetCartItemsAsync()
    {
        var client = await GetClientAsync();

        var response = await client.GetFromJsonAsync<IEnumerable<CartItem>>($"{_apiBaseUrl}/api/Cart");

        return response ?? Array.Empty<CartItem>();
    }

    public async Task<long> CreateCartAsync(CartItemRequest request)
    {
        var client = await GetClientAsync();
        var response = await client.PostAsJsonAsync($"{_apiBaseUrl}/api/cartitem", request);

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadFromJsonAsync<CartCreationResponse>();
            return responseBody.cartItemId;
        }

        return 0;
    }

    public async Task<bool> DeleteCartItemAsync(long id)
    {
        var client = await GetClientAsync();

        var response = await client.DeleteAsync($"{_apiBaseUrl}/api/Cart/{id}");

        return response.IsSuccessStatusCode;
    }

    public async Task<CartItem> GetCartItemAsync(long id)
    {
        var client = await GetClientAsync();

        var response = await client.GetFromJsonAsync<CartItem>($"{_apiBaseUrl}/api/Cart/{id}");

        return response;
    }

    public async Task<bool> ClearCart()
    {
        var client = await GetClientAsync();

        var response = await client.DeleteAsync($"{_apiBaseUrl}/api/Cart");

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateCartItem(long id, int quantity)
    {
        var client = await GetClientAsync();
        var response = await client.PutAsJsonAsync($"{_apiBaseUrl}/api/Cart/{id}", new CartItemRequest{ Quantity = quantity });
        return response.IsSuccessStatusCode;
    }
    public class CartCreationResponse
    {
        public long cartItemId { get; set; }
        public string Message { get; set; }
    }
}
