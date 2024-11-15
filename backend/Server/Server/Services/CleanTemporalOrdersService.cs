using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Services;

public class CleanTemporalOrdersService : BackgroundService
{
    private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(1);
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
                var unitOfWork = scope.ServiceProvider.GetRequiredService<UnitOfWork>();
                try {
                    Console.WriteLine("Ejecutando servicio en segundo plano");
                    List<TemporalOrder> expiredOrders = (List<TemporalOrder>)await unitOfWork.TemporalOrderRepository.GetExpiredOrders(DateTime.UtcNow);

                    foreach (TemporalOrder temporalOrder in expiredOrders)
                    {
                        // Se desasocia la entidad existente del contexto antes de tocar otra
                        var existingEntity = await unitOfWork.TemporalOrderRepository.GetByIdAsync(temporalOrder.Id);
                        if (existingEntity != null)
                        {
                            unitOfWork.Context.Entry(existingEntity).State = EntityState.Detached;
                        }

                        unitOfWork.TemporalOrderRepository.Delete(temporalOrder);

                        ShoppingCart cart = await unitOfWork.ShoppingCartRepository.GetAllShoppingCartByShoppingCartIdAsync(temporalOrder.ShoppingCartId);
                        unitOfWork.ShoppingCartRepository.Update(cart);

                        List<CartContent> cartContents = (List<CartContent>)cart.CartContent;
                        foreach (CartContent cartContent in cartContents)
                        {
                            Product product = await unitOfWork.ProductRepository.GetByIdAsync(cartContent.ProductId);
                            product.Stock += cartContent.Quantity;
                            unitOfWork.ProductRepository.Update(product);

                        }
                    }

                    await unitOfWork.SaveAsync();
                } catch(Exception e)
                {
                    Console.WriteLine($"Error durante la ejecución del servicio: {e.ToString()}");
                }
                
            }
            

            await Task.Delay(_cleanupInterval, stoppingToken);
        }
    }
}
