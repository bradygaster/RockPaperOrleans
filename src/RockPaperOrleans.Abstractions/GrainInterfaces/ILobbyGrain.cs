namespace RockPaperOrleans.Abstractions;

public interface ILobbyGrain : IGrainWithGuidKey
{
    Task SignIn(Player player);
    Task SignOut(Player player);
    Task EnterLobby(Player player);
    Task EnterGame(Player player);
    Task<List<Player>> GetPlayersInLobby();
}
