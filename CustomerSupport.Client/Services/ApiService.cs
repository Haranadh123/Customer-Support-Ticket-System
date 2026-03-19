using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using CustomerSupport.Shared.DTOs;
using CustomerSupport.Shared.Enums;

namespace CustomerSupport.Client.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private string? _token;

    public ApiService()
    {
        // Using HTTP for local development to avoid SSL certificate issues
        _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5119/") };
    }

    public void SetToken(string token)
    {
        _token = token;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public void Logout()
    {
        _token = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<LoginResponse?> LoginAsync(string username, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Auth/login", new LoginRequest(username, password));
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (result != null) SetToken(result.Token);
            return result;
        }
        return null;
    }

    public async Task<List<TicketDto>> GetTicketsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<TicketDto>>("api/Tickets") ?? new();
    }

    public async Task<TicketDetailsDto?> GetTicketDetailsAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<TicketDetailsDto>($"api/Tickets/{id}");
    }

    public async Task<bool> CreateTicketAsync(CreateTicketRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Tickets", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> AssignTicketAsync(int ticketId, int adminId)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/Tickets/{ticketId}/assign", new AssignTicketRequest(adminId));
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateStatusAsync(int ticketId, TicketStatus status)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/Tickets/{ticketId}/status", new UpdateStatusRequest(status));
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> AddCommentAsync(int ticketId, string text, bool isInternal)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/Comments/{ticketId}", new AddCommentRequest(text, isInternal));
        return response.IsSuccessStatusCode;
    }

    public async Task<List<AdminDto>> GetAdminsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<AdminDto>>("api/Tickets/admins") ?? new();
    }
}
