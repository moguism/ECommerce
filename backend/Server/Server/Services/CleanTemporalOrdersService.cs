using Server.Models;

namespace Server.Services;

public class CleanTemporalOrdersService : BackgroundService
{
    private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(5);
    private readonly IServiceProvider _serviceProvider;

    public CleanTemporalOrdersService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetService<UnitOfWork>();
                try {
                    Console.WriteLine("Ejecutando servicio en segundo plano");
                    List<TemporalOrder> expiredOrders = (List<TemporalOrder>)await unitOfWork.TemporalOrderRepository.GetExpiredOrders(DateTime.UtcNow);

                    foreach (TemporalOrder temporalOrder in expiredOrders)
                    {
                        unitOfWork.TemporalOrderRepository.Delete(temporalOrder);

                        ShoppingCart cart = await unitOfWork.ShoppingCartRepository.GetAllShoppingCartByShoppingCartIdAsync(temporalOrder.ShoppingCartId);
                        unitOfWork.ShoppingCartRepository.Update(cart);

                        List<CartContent> cartContents = (List<CartContent>)cart.CartContent;
                        foreach (CartContent cartContent in cartContents)
                        {
                            Product product = await unitOfWork.ProductRepository.GetFullProductById(cartContent.ProductId);
                            product.Stock += cartContent.Quantity;
                            unitOfWork.ProductRepository.Update(product);
                        }
                    }

                    await unitOfWork.SaveAsync();
                } catch(Exception e)
                {
                    Console.WriteLine($"Error durante la ejecución del servicio: {e.Message}");
                }
                
            }
            

            await Task.Delay(_cleanupInterval, stoppingToken);
        }
    }
}
