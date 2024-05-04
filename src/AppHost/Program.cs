using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage").RunAsEmulator();
var clustering = storage.AddTables("clustering");
var grainStorage = storage.AddBlobs("grainstorage");

var orleans = builder.AddOrleans("orleans-cluster")
    .WithClustering(clustering)
    .WithGrainStorage("Lobby", grainStorage)
    .WithGrainStorage("Games", grainStorage)
    .WithGrainStorage("Players", grainStorage);

builder.AddProject<Projects.GameController>("gamecontroller")
       .WithReference(orleans);

builder.AddProject<Projects.Leaderboard>("leaderboard")
       .WithReference(orleans)
       .WithExternalHttpEndpoints();

builder.AddProject<Projects.RoundRobin>("roundrobin").WithReference(orleans);
builder.AddProject<Projects.MrPresumptive>("mrpresumptive").WithReference(orleans);
//builder.AddProject<Projects.Rando>("rando").WithReference(orleans);
//builder.AddProject<Projects.Players>("players").WithReference(orleans);
//builder.AddProject<Projects.Rascals>("rascals").WithReference(orleans);

builder.Build().Run();
