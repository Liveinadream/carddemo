using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Refit;


public partial class NetworkManager : Singleton<NetworkManager>
{

    // 服务器基础地址
    [Export]
    public string BaseUrl = "https://api.example.com"; // 替换为你的服务器地址

    private IGameApi _gameApi;
    public IGameApi GameApi => _gameApi;

    public override void _Ready()
    {
        // 初始化 Refit
        _gameApi = RestService.For<IGameApi>(BaseUrl);
        GD.Print($"[NetworkManager] Initialized with {BaseUrl}");
    }

    /// <summary>
    /// 示例：登录并获取用户信息
    /// </summary>
    public async Task<PlayerProfile> LoginAndGetProfile(string username, string password)
    {
        try
        {
            // 1. 登录
            var loginData = new Dictionary<string, string> 
            { 
                { "username", username }, 
                { "password", password } 
            };
            var token = await _gameApi.LoginAsync(loginData);
            GD.Print($"[NetworkManager] Login Success. Token: {token}");

            // 2. 获取个人信息
            var profile = await _gameApi.GetProfileAsync($"Bearer {token}");
            GD.Print($"[NetworkManager] Welcome back, {profile.Name} (Level {profile.Level})");
            
            return profile;
        }
        catch (ApiException e)
        {
            GD.PrintErr($"[NetworkManager] API Error: {e.StatusCode} - {e.Content}");
            return null;
        }
        catch (Exception e)
        {
            GD.PrintErr($"[NetworkManager] Unexpected Error: {e.Message}");
            return null;
        }
    }

    /// <summary>
    /// 示例：获取所有卡牌数据
    /// </summary>
    public async Task<List<CardInfo>> FetchAllCards()
    {
        try
        {
            var cards = await _gameApi.GetAllCardsAsync();
            GD.Print($"[NetworkManager] Fetched {cards.Count} cards.");
            return cards;
        }
        catch (Exception e)
        {
            GD.PrintErr($"[NetworkManager] Failed to fetch cards: {e.Message}");
            return new List<CardInfo>();
        }
    }
}
