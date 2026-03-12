using System.Net.Http.Json;
using CrusadeTracker.Application.Battles.DTOs;

namespace CrusadeTracker.Web.Services;

public class BattleService
{
    private readonly HttpClient _httpClient;

    public BattleService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<BattleResponse>?> GetBattlesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<BattleResponse>>("api/battles");
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<BattleResponse?> GetBattleAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<BattleResponse>($"api/battles/{id}");
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<BattleResponse?> CreateBattleAsync(DateTimeOffset date, string mission)
    {
        var request = new CreateBattleRequest(date, mission);
        var response = await _httpClient.PostAsJsonAsync("api/battles", request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<BattleResponse>();
        }

        return null;
    }

    public async Task<BattleResponse?> AddParticipantAsync(Guid battleId, Guid forceId, string? forceNameSnapshot = null)
    {
        var request = new AddParticipantRequest(forceId, forceNameSnapshot);
        var response = await _httpClient.PostAsJsonAsync($"api/battles/{battleId}/participants", request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<BattleResponse>();
        }

        return null;
    }

    public async Task<BattleResponse?> SetResultAsync(Guid battleId, Guid forceId, string result)
    {
        var request = new SetResultRequest(forceId, result);
        var response = await _httpClient.PostAsJsonAsync($"api/battles/{battleId}/results", request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<BattleResponse>();
        }

        return null;
    }

    public async Task<BattleResponse?> FinalizeBattleAsync(Guid battleId)
    {
        var response = await _httpClient.PostAsync($"api/battles/{battleId}/finalize", null);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<BattleResponse>();
        }

        return null;
    }
}
