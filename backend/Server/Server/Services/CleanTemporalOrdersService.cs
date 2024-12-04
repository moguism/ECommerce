using Microsoft.EntityFrameworkCore;
using Nethereum.Util;
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
                    List<TemporalOrder> expiredOrders = (List<TemporalOrder>)await unitOfWork.TemporalOrderRepository.GetFullTemporalOrders();

                    foreach (TemporalOrder temporalOrder in expiredOrders)
                    {
                        if(temporalOrder.ExpirationDate < DateTime.UtcNow)
                        {
                            unitOfWork.TemporalOrderRepository.Delete(temporalOrder);
                            foreach (ProductsToBuy cartContent in temporalOrder.Wishlist.Products)
                            {
                                var product = await unitOfWork.ProductRepository.GetByIdAsync(cartContent.ProductId);
                                //unitOfWork.Context.Entry(cartContent.Product).State = EntityState.Detached;
                                product.Stock += cartContent.Quantity;
                                unitOfWork.ProductRepository.Update(product);
                                /*unitOfWork.ProductsToBuyRepository.Delete(cartContent);*/
                            }
                        }
                        // Se desasocia la entidad existente del contexto antes de tocar otra
                        /*var existingEntity = temporalOrder;
                        if (existingEntity != null)
                        {
                            unitOfWork.Context.Entry(existingEntity).State = EntityState.Detached;
                            
                        }

                        unitOfWork.TemporalOrderRepository.Delete(temporalOrder);*/

                        
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
