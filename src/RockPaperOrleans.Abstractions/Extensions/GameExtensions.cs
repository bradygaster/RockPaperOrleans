namespace RockPaperOrleans.Abstractions;

public static class GameExtensions
{
    public static double CalculateWinPercentage(this Player player)
    {
        try
        {
            double pct = (double)player.WinCount / (double)player.TotalGamesPlayed;
            return Math.Round(pct * 100, 0);
        }
        catch
        {
            return 0;
        }
    }

    public static bool HasPlayers(this Game game)
    {
        return !(game.Player1 == null && game.Player2 == null);
    }

    public static bool IsGameComplete(this Game game )
    {
        return game.Rounds > game.Turns.Count;
    }

    public static string? ScoreTurn(this Turn turn)
    {
        // if both players through X, tie game
        if (turn.Throws[0].Play == turn.Throws[1].Play)
            return "Tie";

        // who threw what
        var rock = turn.Throws.FirstOrDefault(x => x.Play == Play.Rock);
        var paper = turn.Throws.FirstOrDefault(x => x.Play == Play.Paper);
        var scissors = turn.Throws.FirstOrDefault(x => x.Play == Play.Scissors);

        return (rock, paper, scissors) switch
        {
			// paper covers rock
			{ paper: not null, rock: not null } => paper.Player,

			// rock breaks scissors
			{ rock: not null, scissors: not null } => rock.Player,

			// scissors cut paper
			{ scissors: not null, paper: not null } => scissors.Player,

			// no idea how we'd ever get here
			_ => null
		};
    }
}