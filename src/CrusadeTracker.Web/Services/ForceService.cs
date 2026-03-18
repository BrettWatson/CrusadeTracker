using System.Net.Http.Json;
using CrusadeTracker.Application.Forces.DTOs;
using CrusadeTracker.Application.Units.DTOs;

namespace CrusadeTracker.Web.Services;

public class ForceService
{
    private readonly HttpClient _httpClient;

    public ForceService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ForceResponse>?> GetForcesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<ForceResponse>>("api/forces");
    }

    public async Task<ForceDetailResponse?> GetForceAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<ForceDetailResponse>($"api/forces/{id}");
    }

    public async Task<ForceResponse?> CreateForceAsync(string name, string faction, int supplyLimit)
    {
        var request = new CreateForceRequest(name, faction, supplyLimit);
        var response = await _httpClient.PostAsJsonAsync("api/forces", request);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<ForceResponse>();
    }

    public async Task<ForceDetailResponse?> ImportForceAsync(string battleForgeExport)
    {
        var request = new ImportForceRequest(battleForgeExport);
        var response = await _httpClient.PostAsJsonAsync("api/forces/import", request);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<ForceDetailResponse>();
    }

    public async Task<ForceResponse?> UpdateForceAsync(Guid id, string name, string faction)
    {
        var request = new UpdateForceRequest(name, faction);
        var response = await _httpClient.PutAsJsonAsync($"api/forces/{id}", request);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<ForceResponse>();
    }

    public async Task<bool> DeleteForceAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/forces/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<UnitResponse?> AddUnitAsync(Guid forceId, string name, string dataSheet, int points)
    {
        var request = new AddUnitRequest(name, dataSheet, points);
        var response = await _httpClient.PostAsJsonAsync($"api/forces/{forceId}/units", request);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<UnitResponse>();
    }

    public async Task<bool> RemoveUnitAsync(Guid forceId, Guid unitId)
    {
        var response = await _httpClient.DeleteAsync($"api/forces/{forceId}/units/{unitId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<UnitResponse?> AddBattleHonourAsync(Guid forceId, Guid unitId, string honour)
    {
        var request = new AddBattleHonourRequest(honour);
        var response = await _httpClient.PostAsJsonAsync($"api/forces/{forceId}/units/{unitId}/honours", request);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<UnitResponse>();
    }

    public async Task<bool> RemoveBattleHonourAsync(Guid forceId, Guid unitId, string honour)
    {
        var response = await _httpClient.DeleteAsync($"api/forces/{forceId}/units/{unitId}/honours/{Uri.EscapeDataString(honour)}");
        return response.IsSuccessStatusCode;
    }

    public async Task<UnitResponse?> AddBattleScarAsync(Guid forceId, Guid unitId, string scar)
    {
        var request = new AddBattleScarRequest(scar);
        var response = await _httpClient.PostAsJsonAsync($"api/forces/{forceId}/units/{unitId}/scars", request);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<UnitResponse>();
    }

    public async Task<bool> RemoveBattleScarAsync(Guid forceId, Guid unitId, string scar)
    {
        var response = await _httpClient.DeleteAsync($"api/forces/{forceId}/units/{unitId}/scars/{Uri.EscapeDataString(scar)}");
        return response.IsSuccessStatusCode;
    }
}
