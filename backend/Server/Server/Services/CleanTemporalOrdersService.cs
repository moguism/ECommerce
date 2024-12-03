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
                    List<TemporalOrder> expiredOrders = (List<TemporalOrder>)await unitOfWork.TemporalOrderRepository.GetExpiredOrders(DateTime.UtcNow.AddMinutes(5));

                    foreach (TemporalOrder temporalOrder in expiredOrders)
                    {
                        // Se desasocia la entidad existente del contexto antes de tocar otra
                        var existingEntity = await unitOfWork.TemporalOrderRepository.GetFullTemporalOderByIdWithoutUser(temporalOrder.Id);
                        if (existingEntity != null)
                        {
                            unitOfWork.Context.Entry(existingEntity).State = EntityState.Detached;
                        }

                        unitOfWork.TemporalOrderRepository.Delete(temporalOrder);

                        foreach (ProductsToBuy cartContent in existingEntity.Wishlist.Products)
                        {
                            Product product = cartContent.Product;
                            product.Stock += cartContent.Quantity;
                            unitOfWork.ProductRepository.Update(product);
                            /*unitOfWork.ProductsToBuyRepository.Delete(cartContent);*/
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
