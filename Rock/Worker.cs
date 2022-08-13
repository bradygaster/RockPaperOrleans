using Orleans;
using RockPaperOrleans.Abstractions;

namespace Rock
{
    public class Worker : BackgroundService
    {
        public IPlayerGrain PlayerGrain { get; set; }
        public IPlayerObserver PlayerObserver { get; }
        public IGrainFactory GrainFactory { get; set; }
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger,
            IPlayerObserver playerObserver,
            IGrainFactory grainFactory)
        {
            _logger = logger;
            PlayerObserver = playerObserver;
            GrainFactory = grainFactory;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            PlayerGrain = GrainFactory.GetGrain<IPlayerGrain>(nameof(Rock));
            var reference = await GrainFactory.CreateObjectReference<IPlayerObserver>(PlayerObserver);
            await PlayerGrain.SignIn(reference);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            var reference = await GrainFactory.CreateObjectReference<IPlayerObserver>(PlayerObserver);
            await PlayerGrain.SignOut();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}