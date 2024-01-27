using GoodsApi.Data.Entities;


namespace Goods.Services;

public class ProductService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly SessionService _sessionService;
    private string _apiBaseUrl;
    public ProductService(IHttpClientFactory clientFactory, SessionService sessionService)
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

    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        var client = _clientFactory.CreateClient();

        var response = await client.GetFromJsonAsync<IEnumerable<Product>>($"{_apiBaseUrl}/api/Product");

        return response ?? Array.Empty<Product>();
    }

    public async Task<Product> GetProductByIdAsync(long id)
    {
        var client = _clientFactory.CreateClient();

        var response = await client.GetFromJsonAsync<Product>($"{_apiBaseUrl}/api/Product/{id}");

        return response;
    }

    public async Task<long> CreateProductAsync(Product product)
    {
        var client = await GetClientAsync();
        var response = await client.PostAsJsonAsync($"{_apiBaseUrl}/api/Products", product);

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadFromJsonAsync<ProductCreationResponse>();
            return responseBody.ProductId;
        }

        return 0;
    }

    public async Task<bool> UpdateProductAsync(long id, Product product)
    {
        var client = await GetClientAsync();

        var response = await client.PutAsJsonAsync($"{_apiBaseUrl}/api/Product/{id}", product);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteProductAsync(long id)
    {
        var client = await GetClientAsync();

        var response = await client.DeleteAsync($"{_apiBaseUrl}/api/Product/{id}");

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UploadImageAsync(long productId, IFormFile imageFile, string imageType)
    {
        var client = await GetClientAsync();

        var formData = new MultipartFormDataContent();
        formData.Add(new StreamContent(imageFile.OpenReadStream()), "file", imageFile.Name);
        formData.Add(new StringContent(imageType), "imageType");

        var response = await client.PostAsync($"{_apiBaseUrl}/api/Product/UploadImage/{productId}", formData);

        return response.IsSuccessStatusCode;
    }
    public class ProductCreationResponse
    {
        public long ProductId { get; set; }
        public string Message { get; set; }
    }
}
