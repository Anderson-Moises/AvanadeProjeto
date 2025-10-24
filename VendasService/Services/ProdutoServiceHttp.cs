using System.Net.Http.Headers;
using System.Net.Http.Json;
using VendasService.Dtos;

namespace VendasService.Services
{
    public class ProdutoServiceHttp : IProdutoService
    {
        private readonly HttpClient _httpClient;

        public ProdutoServiceHttp(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ProdutoDto>> ListarAsync(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/produtos");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var produtos = await response.Content.ReadFromJsonAsync<IEnumerable<ProdutoDto>>();
            return produtos ?? Enumerable.Empty<ProdutoDto>();
        }

        public async Task<ProdutoDto?> ObterPorIdAsync(int id, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/produtos/{id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<ProdutoDto>();
        }

        public async Task<bool> RemoverAsync(int id, int quantidade, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/produtos/{id}?quantidade={quantidade}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ReporInterno(int produtoId, int quantidade, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"api/produtos/{produtoId}/repor?quantidade={quantidade}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}