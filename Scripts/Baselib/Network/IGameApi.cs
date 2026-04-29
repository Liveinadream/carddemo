using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;


public record PlayerProfile(string Id, string Name, int Level, int Gold);
public record CardInfo(string Id, string Name, string Description, int Attack, int Health);

public interface IGameApi
{
    [Get("/player/profile")]
    Task<PlayerProfile> GetProfileAsync([Header("Authorization")] string token);

    [Get("/cards/all")]
    Task<List<CardInfo>> GetAllCardsAsync();

    [Post("/player/login")]
    Task<string> LoginAsync([Body] Dictionary<string, string> loginData);
}
