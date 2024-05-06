namespace RockPaperOrleans.Abstractions;

public interface IMatchmakingGrain : IGrainWithGuidKey
{
    Task<(Player One, Player Two)?> ChoosePlayers();
}
