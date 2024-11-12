namespace Server.Services;

public class CleanTemporalOrdersService : BackgroundService
{
    private readonly TemporalOrderService _temporalOrderService;
    private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(1);

    public CleanTemporalOrdersService(TemporalOrderService temporalOrderService)
    {
        _temporalOrderService = temporalOrderService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine("Ejecutando servicio en segundo plano");
            await _temporalOrderService.RemoveExpiredOrders();
            await Task.Delay(_cleanupInterval, stoppingToken);
        }
    }
}
