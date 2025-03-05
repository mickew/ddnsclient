namespace DdnsClient.Services
{
    internal class DdnsUpdateService : IHostedService, IDisposable
    {
        private const int Oneday = 24 * 60 * 60;
        private int _secondsCounter = 24 * 60 * 60;
        private bool _isWorking = false;
        private readonly ILogger<DdnsUpdateService> _logger;
        private readonly IDdnsService _ddnsService;
        private readonly int _interval;
        private Timer? _timer;

        public DdnsUpdateService(ILogger<DdnsUpdateService> logger, IConfiguration configuration, IDdnsService ddnsService)
        {
            _logger = logger;
            _interval = configuration.GetValue<int>("Ddns:CheckEverySec", 300);
            _ddnsService = ddnsService;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("DdnsUpdateService running at: {time}", DateTimeOffset.Now);
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(_interval));
            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            if (!_isWorking && _secondsCounter >= Oneday)
            {
                _isWorking = true;
                await _ddnsService.GetIpFromDdnsAsync();
                _isWorking = false;
                _secondsCounter = 0;
            }
            _secondsCounter += _interval;
            if (_isWorking)
            {
                return;
            }
            _isWorking = true;
            _logger.LogInformation("DdnsUpdateService working at: {time}", DateTimeOffset.Now);
            await _ddnsService.UpdateAsync();
            _isWorking = false;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("DdnsUpdateService is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
