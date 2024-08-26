using System.Net;
using CloudCustomers.API.config;
using CloudCustomers.API.Models;
using Microsoft.Extensions.Options;

namespace CloudCustomers.API.Services;

public interface IUsersService
{
    public Task<List<User>> GetAllUsers();
}
public class UsersService : IUsersService
{
    private readonly HttpClient _httpClient;
    private readonly UsersApiOptions _apiConfig;
    public UsersService(HttpClient httpClient, IOptions<UsersApiOptions> apiConfig)
    {
        _httpClient = httpClient;
        _apiConfig = apiConfig.Value;
    }


    public async Task< List<User>> GetAllUsers()
    {
        var usersResponse = await _httpClient.GetAsync(_apiConfig.Endpoint);
        if (usersResponse.StatusCode == HttpStatusCode.NotFound)
        {
            return new List<User>();
        }
        
        var responseContent = usersResponse.Content;
        var responseContent2 = await usersResponse.Content.ReadAsStringAsync();
        Console.WriteLine("User content is " + responseContent2);

        var allUsers = await responseContent.ReadFromJsonAsync<List<User>>();
        return allUsers.ToList();
    }
}