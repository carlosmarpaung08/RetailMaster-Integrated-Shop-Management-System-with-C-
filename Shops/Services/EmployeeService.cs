using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shops.Models;
using Shops_API.Models;

public class EmployeeService
{
    private readonly HttpClient _httpClient;

    public EmployeeService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<List<EmployeeViewModel>> GetEmployees()
    {
        var response = await _httpClient.GetAsync("https://localhost:7268/api/Employees");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var employees = JsonConvert.DeserializeObject<List<EmployeeViewModel>>(content);
            return employees;
        }
        else
        {
            // Handle error response
            throw new HttpRequestException($"Failed to get employees. Status code: {response.StatusCode}");
        }
    }

    public async Task<EmployeeViewModel> GetEmployee(int id)
    {
        var response = await _httpClient.GetAsync($"https://localhost:7268/api/Employees/{id}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var employee = JsonConvert.DeserializeObject<EmployeeViewModel>(content);
            return employee;
        }
        else
        {
            // Handle error response
            throw new HttpRequestException($"Failed to get employee. Status code: {response.StatusCode}");
        }
    }

    public async Task<EmployeeViewModel> CreateEmployee(EmployeeViewModel employee)
    {
        var json = JsonConvert.SerializeObject(employee);
        var response = await _httpClient.PostAsync("https://localhost:7268/api/Employees", new StringContent(json, System.Text.Encoding.UTF8, "application/json"));

        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            // Handle the exception here, for example, by logging the error or displaying a message to the user.
            Console.WriteLine($"Failed to create employee. Error: {ex.Message}");
            throw; // Re-throw the exception to propagate it further if needed.
        }

        var content = await response.Content.ReadAsStringAsync();
        var createdEmployee = JsonConvert.DeserializeObject<EmployeeViewModel>(content);
        return createdEmployee;
    }

    public async Task<EmployeeViewModel> UpdateEmployee(int id, EmployeeViewModel employee)
    {
        var json = JsonConvert.SerializeObject(employee);
        var response = await _httpClient.PutAsync($"https://localhost:7268/api/Employees/{id}", new StringContent(json, System.Text.Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var updatedEmployee = JsonConvert.DeserializeObject<EmployeeViewModel>(content);
        return updatedEmployee;
    }

    public async Task DeleteEmployee(int id)
    {
        var response = await _httpClient.DeleteAsync($"https://localhost:7268/api/Employees/{id}");
        response.EnsureSuccessStatusCode();
    }
}
