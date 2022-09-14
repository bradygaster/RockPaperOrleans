# Rock, Paper, Orleans

Rock, Paper, Orleans (RPO) is a game built using dotnet, Orleans, and runs in Azure. The idea behind RPO is that you write a "player bot" in which you implement your player logic. The game engine essentially runs forever as a dotnet Worker service (which we may tweak later to run as an Orleans timer). Players wait in a lobby, and are matchmade each round randomly. The game is played, the winner and loser recorded, and the next game started. 

RPO is hosted by Brady Gaster at https://rockpaperorleans.net. 



## Prerequisites

To get this project up and running on your own machine and in your own Azure subscription, you'll need the following things:

* An Azure subscription (sign up here for free).
* dotnet 6.0 on your machine.
* The Azure CLI installed.
* The Azure Dev CLI, as this project makes use of the exciting new updates offered by `azd`. 
* Either the Azure Storage Emulator or Azure Cosmos DB emulator on your development machine.
* Ideally, you'll have Visual Studio Preview or Visual Studio Code to make tweaks and - hopefully - write and contribute your own player to the RPO ecosystem. 



## Getting Started

Here's what RPO looks like when you have it up and running. In this screenshot, we're using the dark theme, and we have all the default players (at the time of v1's release) running in the app. 

![Rock, Paper, Orleans running in the browser.](docs\media\rpo.png)



The game topology consists of a series of Visual Studio projects you'll find in the `src` folder. Here's a breakout of these projects so you understand what's what. 

| Project          | Purpose                                                      |
| ---------------- | ------------------------------------------------------------ |
| GameController   | The dotnet worker service project that hosts the game engine, or the "game loop". This project's front door is the Orleans Dashboard, as this is the one silo in the solution that hosts Grains. |
| Leaderboard      | This is the front-end UI of the app.                         |
| Players          | Hosts the more basic players in the app.                     |
| Rando            | Hosts the "random" player and the "slow random" player, which emulates a player taking a longer period of time to perform their move. Also contains the Captain Obvious player, which demonstrates implementing a simple logic flow in a player's implementation. |
| .Abstractions    | This project contains the various Grain interfaces or abstractions, as well as any models (the "nouns" in the system as well as the behavioral abstractions) are contained here. |
| .Grains          | Implementations of the Grain interfaces.                     |
| RockPaperOrleans | Infrastructure and convenience classes that simplify the programming model of building and hosting players. |



## Running the app

Whether you run the app using Visual Studio, VS Code, Tye, or you just deploy it first, understand the order in which the services must start. 

1. Game Controller
2. Leaderboard
3. Any players



If you don't start things up in this order, well - it won't work. At the moment, the system is also set up such that each service should be run as a single-instance. You'd only need compute instance of each service running. If you scale them out, well - that might be weird, too. You're at your leisure to try but... yeah. Moving on!



## Deploying the app

We used AZD to make this process easier. You'll find that we adhered to the AZD-friendly layout for the `infra` parts, where our Bicep files and ARM parameter files are located. This makes deployment as simple as:

1. `azd provision -e rpo`
2. `azd deploy --service gamecontroller`
3. Click the link written to the screen once the `gamecontroller` app is deployed to open up the RPO Orleans Dashboard, where you can see all the silos and grains as the additional services appear.
4. `azd deploy --service leaderboard`
5. Click the link written to screen once the `leaderboard` app is deployed to open up the RPO leaderboard, or main user interface. Leave the leaderboard open as you run the next two AZD commands, and you'll see players appear as the apps start up in Azure. 
6. `azd deploy --service players`
7. `azd deploy --service rando`
8. Watch as the game loop runs and players win and lose and move up and down in the leaderboard.



Or, you can just do it all in one step and open the links as they're written to screen with `azd up`. 



## Writing a player

To write a player, you create a new class that inherits from `PlayerBase`. You can override any of the methods, but you're only required to implement the `Go()` method. `Go` is where all the logic for your player is stored. 

Let's walk through some of the players we shipped with v1 of RPO give you an overview of how to write a player bot, and then later, how to host it (don't worry, it's simple). 



### Players Examples

In the `Players` project, there are a series of very-basic players that behave in very-obvious ways. 



```csharp
public class AlwaysPaper : PlayerBase
{
    public AlwaysPaper(ILogger<AlwaysPaper> logger) : base(logger) { }

    public override Task<Play> Go()
        => Task.FromResult(Play.Paper);
}

public class AlwaysRock : PlayerBase
{
    public AlwaysRock(ILogger<AlwaysRock> logger) : base(logger) { }

    public override Task<Play> Go()
        => Task.FromResult(Play.Rock);
}

public class AlwaysScissors : PlayerBase
{
    public AlwaysScissors(ILogger<AlwaysScissors> logger) : base(logger) { }

    public override Task<Play> Go()
        => Task.FromResult(Play.Scissors);
}
```



### Rando Examples

The `Rando` project contains, obviously, the `Rando` player, which, probably-also-obviously, throws random moves. Literally random. He has a pal named `SlowRando` that is built to show how the game loop continues to work fine even when a bot is slow. 

> One day we'll implement logic to control a bot that just stops or is too slow, but for now we'll control that using code reviews.

The code for these simple players is, again, probably pretty obvious in nature. 

```csharp
public class Rando : PlayerBase
{
    public Rando(ILogger<Rando> logger) : base(logger) { }  
    public override Task<Play> Go() => Task.FromResult((Play)Random.Shared.Next(0, 3));
}

// simulate a player taking a few seconds to run
public class SlowRando : PlayerBase
{
    public SlowRando(ILogger<Rando> logger) : base(logger) { }
    public override async Task<Play> Go()
    {
        await Task.Delay(Random.Shared.Next(250, 1000));
        return (Play)Random.Shared.Next(0, 3);
    }
}
```



### Captain Obvious

`CaptainObvious` bases his move off the name of the player, or more generally, a component of the name of the player using very-very-basic pattern matching. 

```csharp
public class CaptainObvious : PlayerBase
{
    private Player _opponent;

    public CaptainObvious(ILogger<CaptainObvious> logger) : base(logger) { }

    public override Task OnOpponentSelected(Player player, Player opponent)
    {
        _opponent = opponent;
        return base.OnOpponentSelected(player, opponent);
    }

    public override Task<Play> Go()
    {
        var result = (_opponent) switch
        {
            Player _ when _opponent.Name.ToLower().Contains("scissors") => Play.Rock,
            Player _ when _opponent.Name.ToLower().Contains("rock") => Play.Paper,
            Player _ when _opponent.Name.ToLower().Contains("paper") => Play.Scissors,
            _ => (Play)Random.Shared.Next(0, 3)
        };

        return Task.FromResult(result);
    }
}
```



## Hosting a player

Both `Rando` and `Players` show examples of how to host multiple players in one Kestrel host. We've provided the extension method `PlayRockPaperScissors` you can use on the `ISiloBuilder` of your own host code, and the `EnlistPlayer<T>` method which takes any `PlayerBase` inheritor. 

That means you can host multiple players, like our `Players` host:

```csharp
IHost host = Host.CreateDefaultBuilder(args)
    .UseOrleans((context, siloBuilder) =>
    {
        siloBuilder
            .PlayRockPaperOrleans(context.Configuration)
            .EnlistPlayer<AlwaysPaper>()
            .EnlistPlayer<AlwaysRock>()
            .EnlistPlayer<AlwaysScissors>();
    })
    .ConfigureServices((services) =>
    {
        services.AddWorkerAppApplicationInsights("Players Silo");
    })
    .Build();
```



Or, you can write your own worker service project and configure it to host your own player. 

```csharp
IHost host = Host.CreateDefaultBuilder(args)
    .UseOrleans((context, siloBuilder) =>
    {
        siloBuilder
            .PlayRockPaperOrleans(context.Configuration)
            .EnlistPlayer<YourAwesomeCustomPlayer>()
    })
    .ConfigureServices((services) =>
    {
        services.AddWorkerAppApplicationInsights("Custom Player Silo");
    })
    .Build();
```





## A few things to know...

As with any codebase, there are a few things you should know before you dive in, and especially, before you deploy. Nothing big, just, a few notes. 

### Changing the data provider

In `SiloBuilderExtensions.cs`, we've defaulted to using the Azure Storage providers for both clustering and grain storage. If you'd prefer to use Cosmos DB, just swap these lines of code, and make sure your Cosmos DB emulator is running and that each of the projects' `appsettings.json` are configured appropriately. 

### Logs

RPO has a ton of logs to aid with development-time and debugging. You'll want to update the `appsettings.json` to "turn down the log verbosity" to warning or lower once you're ready to deploy "for real" or you'll have a LOT of logs. 



## Your bot can do better!

We'd like you to write your own player, so feel free to send a pull request. When you do, please either add your player to the `Players` project, or write your own host. If you do, make sure to set up your own ports for the silo and gateway ports for your host, and add your service to the `azure.yaml` file and corresponding Bicep/JSON files to the `infra/modules` folder. In addition, please add your own workflow in the `.github` folder, so we can deploy your player separately if you're hosting it on your own. 