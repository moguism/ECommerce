using Server.Models;

namespace Server.Services;

public class CleanTemporalOrdersService : BackgroundService
{
    private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(5);

    /*public CleanTemporalOrdersService(TemporalOrderService temporalOrderService)
    {
        _temporalOrderService = temporalOrderService;
    }*/

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        FarminhouseContext farminhouseContext = new FarminhouseContext();
        UnitOfWork unitOfWork = new UnitOfWork(farminhouseContext);
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine("Ejecutando servicio en segundo plano");
            List<TemporalOrder> expiredOrders = (List<TemporalOrder>) await unitOfWork.TemporalOrderRepository.GetExpiredOrders(DateTime.UtcNow);

            foreach (TemporalOrder temporalOrder in expiredOrders)
            {
                unitOfWork.TemporalOrderRepository.Delete(temporalOrder);
                
                ShoppingCart cart = await unitOfWork.ShoppingCartRepository.GetFullByIdAsync(temporalOrder.ShoppingCartId);
                cart.Temporal = false;
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

            await Task.Delay(_cleanupInterval, stoppingToken);
        }
    }
}
