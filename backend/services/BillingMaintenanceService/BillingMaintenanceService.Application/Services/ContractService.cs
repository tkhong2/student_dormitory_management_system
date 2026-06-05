using BillingMaintenanceService.Application.DTOs;
using BillingMaintenanceService.Application.Interfaces;
using System.Linq;
using System.Net.Http.Json;

namespace BillingMaintenanceService.Application.Services
{
    public class ContractService : IContractService
    {
        private readonly HttpClient _httpClient;

        public ContractService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ContractDto?> GetContractByIdAsync(int contractId)
        {
            var response = await _httpClient.GetAsync($"api/contracts/{contractId}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<ContractDto>();
        }

        public async Task<IEnumerable<ContractDto>> GetContractsByStatusAsync(string status)
        {
            var response = await _httpClient.GetAsync($"api/contracts/status/{status}");
            if (!response.IsSuccessStatusCode)
            {
                return Enumerable.Empty<ContractDto>();
            }

            return await response.Content.ReadFromJsonAsync<IEnumerable<ContractDto>>() ?? Enumerable.Empty<ContractDto>();
        }
    }
}
