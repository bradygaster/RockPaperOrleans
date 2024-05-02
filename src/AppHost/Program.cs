var builder = DistributedApplication.CreateBuilder(args);
var redis = builder.AddRedis("redis");

var orleans = builder.AddOrleans("orleans-cluster")
    .WithClustering(redis)
    .WithGrainStorage("Lobby", redis)
    .WithGrainStorage("Games", redis)
    .WithGrainStorage("Players", redis);

builder.AddProject<Projects.GameController>("gamecontroller")
       .WithReference(orleans);

builder.AddProject<Projects.Leaderboard>("leaderboard")
       .WithReference(orleans);

builder.AddProject<Projects.Players>("players")
       .WithReference(orleans);

builder.AddProject<Projects.Rando>("rando")
       .WithReference(orleans);

builder.AddProject<Projects.Rascals>("rascals")
       .WithReference(orleans);

builder.Build().Run();
