namespace RockPaperOrleans.Grains;

[CollectionAgeLimit(Minutes = 2)]
public class GameGrain(
    [PersistentState("Games", storageName: "Games")] IPersistentState<Game> game,
    ILogger<GameGrain> logger) : Grain, IGameGrain
{
    private IPersistentState<Game> Game { get; set; } = game;

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        Game.State.Id = this.GetPrimaryKey();

        return base.OnActivateAsync(cancellationToken);
    }

    public Task<Game> GetGame()
        => Task.FromResult(Game.State);

    public Task SetGame(Game game)
        => Task.FromResult(Game.State = game);

    public async Task SelectPlayers()
    {
        var game = await GetGame();

        logger.LogInformation("RPO: Getting the matchmaker for Game {GameId}", Game.State.Id);
        var matchmaker = GrainFactory.GetGrain<IMatchmakingGrain>(Guid.Empty);

        logger.LogInformation("RPO: Getting players for Game {GameId}", Game.State.Id);
        var chosenPlayers = await matchmaker.ChoosePlayers();

        if (chosenPlayers is not var (playerOne, playerTwo))
        {
            logger.LogInformation("RPO: There aren't enough players in the lobby to field a game.");
        }
        else
        {
            logger.LogInformation("RPO: Players {PlayerOneName} and {PlayerTwoNameA} selected for Game {GameId}.", playerOne.Name, playerTwo.Name, Game.State.Id);

            // start the game
            game.Player1 = playerOne.Name;
            game.Player2 = playerTwo.Name;
            game.Started = DateTime.UtcNow;
            await SetGame(game);

            // notify the players
            await GrainFactory
                    .GetGrain<IPlayerSessionGrain>(playerOne.Name)
                        .OpponentSelected(playerTwo);

            await GrainFactory
                    .GetGrain<IPlayerSessionGrain>(playerTwo.Name)
                        .OpponentSelected(playerOne);

            // notify the leaderboard
            var leaderBoard = GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty);
            await leaderBoard.GameStarted(game, playerOne, playerTwo);
        }
    }

    public async Task Go()
    {
        var player1Grain = GrainFactory.GetGrain<IPlayerSessionGrain>(Game.State.Player1);
        var player2Grain = GrainFactory.GetGrain<IPlayerSessionGrain>(Game.State.Player2);
        var leaderBoard = GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty);

        if (!(await player1Grain.IsPlayerOnline()
            && await player2Grain.IsPlayerOnline()))
            await ScoreGame();

        var turn = new Turn();
        await leaderBoard.TurnStarted(turn, Game.State);

        var player1Play = await player1Grain.Go();
        var player2Play = await player2Grain.Go();

        turn.Throws.Add(new Throw { Play = player1Play, Player = Game.State.Player1 });
        turn.Throws.Add(new Throw { Play = player2Play, Player = Game.State.Player2 });
        Game.State.Turns.Add(turn);

        await SetGame(Game.State);
        await leaderBoard.TurnCompleted(turn, Game.State);
    }

    public async Task ScoreTurn()
    {
        var turn = Game.State.Turns.Last();
        turn.Winner = turn.ScoreTurn();

        await GrainFactory
                .GetGrain<IPlayerSessionGrain>(Game.State.Player1)
                    .TurnComplete(turn);

        await GrainFactory
                .GetGrain<IPlayerSessionGrain>(Game.State.Player2)
                    .TurnComplete(turn);

        await GrainFactory
                .GetGrain<ILeaderboardGrain>(Guid.Empty)
                    .TurnScored(turn, Game.State);
    }

    public async Task ScoreGame()
    {
        var player1Grain = GrainFactory.GetGrain<IPlayerSessionGrain>(Game.State.Player1);
        var player2Grain = GrainFactory.GetGrain<IPlayerSessionGrain>(Game.State.Player2);
        var player1 = await player1Grain.Get();
        var player2 = await player2Grain.Get();

        var player1WinCount = Game.State.Turns.Count(x => x.Winner == player1.Name);
        var player2WinCount = Game.State.Turns.Count(x => x.Winner == player2.Name);

        logger.LogInformation("RPO: {PlayerOneName} won {PlayerOneWinCount} out of {Rounds} rounds.", player1.Name, player1WinCount, Game.State.Rounds);
        logger.LogInformation("RPO: {PlayerTwoName} won {PlayerTwoWinCount} out of {Rounds} rounds.", player2.Name, player2WinCount, Game.State.Rounds);

        if (player1WinCount > player2WinCount)
        {
            await player1Grain.RecordWin(player2);
            await player2Grain.RecordLoss(player1);
            Game.State.Winner = player1.Name;
            logger.LogInformation("RPO: {PlayerName} wins.", player1.Name);
        }
        if (player2WinCount > player1WinCount)
        {
            await player2Grain.RecordWin(player1);
            await player1Grain.RecordLoss(player2);
            Game.State.Winner = player2.Name;
            logger.LogInformation("RPO: {PlayerName} wins.", player2.Name);
        }
        if (player2WinCount == player1WinCount)
        {
            await player2Grain.RecordTie(player1);
            await player1Grain.RecordTie(player2);
            Game.State.Winner = "Tie";
            logger.LogInformation("RPO: {PlayerOneName} ties with {PlayerTwoName}.", player1.Name, player2.Name);
        }

        await SetGame(Game.State);

        // refresh the win/loss data points
        player1 = await player1Grain.Get();
        player2 = await player2Grain.Get();

        var lobbyGrain = GrainFactory.GetGrain<ILobbyGrain>(Guid.Empty);

        if (await player1Grain.IsPlayerOnline())
            await lobbyGrain.EnterLobby(player1);

        if (await player2Grain.IsPlayerOnline())
            await lobbyGrain.EnterLobby(player2);

        var leaderBoard = GrainFactory.GetGrain<ILeaderboardGrain>(Guid.Empty);
        await leaderBoard.GameCompleted(Game.State);
    }
}
