using Server.DTOs;
using Server.Mappers;
using Server.Models;
using static TorchSharp.torch.utils;

namespace Server.Services
{
    public class TemporalOrderService
    {

        private readonly UnitOfWork _unitOfWork;

        public TemporalOrderService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TemporalOrder> GetFullTemporalOrderByUserId(int id)
        {
            return await _unitOfWork.TemporalOrderRepository.GetFullTemporalOrderByUserId(id);
        }

        
        public async Task<TemporalOrder> CreateTemporalOrder(User user, Wishlist wishlist, bool quick)
        {

            //La añade a la base de datos
            TemporalOrder order = await _unitOfWork.TemporalOrderRepository.InsertAsync(new TemporalOrder
            {
                UserId = user.Id,
                WishlistId = wishlist.Id,
                Wishlist = wishlist,
                ExpirationDate = DateTime.UtcNow,
                Quick = quick
            });


            //Resta el stock a los productos que quiere comprar el usuario
            foreach(ProductsToBuy productToBuy in wishlist.Products)
            {
                Product product = await _unitOfWork.ProductRepository.GetByIdAsync(productToBuy.ProductId);
                product.Stock -= productToBuy.Quantity;
                _unitOfWork.ProductRepository.Update(product);
            }

            await _unitOfWork.SaveAsync();
            return order;
        }

        public async Task<TemporalOrder> GetFullTemporalOrderById(int id)
        {
            return await _unitOfWork.TemporalOrderRepository.GetFullTemporalOrderById(id);
        }

        public async Task UpdateExpiration(TemporalOrder temporalOrder)
        {
            temporalOrder.ExpirationDate = DateTime.UtcNow;
            _unitOfWork.TemporalOrderRepository.Update(temporalOrder);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateTemporalOrder(TemporalOrder temporalOrder)
        {
            _unitOfWork.TemporalOrderRepository.Update(temporalOrder);
            await _unitOfWork.SaveAsync();
        }

        public async Task<TemporalOrder> GetBySessionId(string sessionid)
        {
            TemporalOrder order = await _unitOfWork.TemporalOrderRepository.GetFullTemporalOderByHashOrSession(sessionid);
            return order;
        }

        public async Task<Order> CreateOrderFromTemporal(string hashOrSessionOrder, string hashOrSessionTemporal, int userId, int paymentType)
        {
            Order existingOrder = await _unitOfWork.OrderRepository.GetByHashOrSession(hashOrSessionOrder);
            if (existingOrder != null)
            {
                return existingOrder;
            }

            //Recoge la ultima orden temporal del usuario
            TemporalOrder temporalOrder = await _unitOfWork.TemporalOrderRepository.GetFullTemporalOderByHashOrSession(hashOrSessionTemporal);
            if (temporalOrder == null)
            {
                return null;
            }

            Order order = new Order
            {
                CreatedAt = DateTime.UtcNow,
                PaymentTypeId = paymentType,
                //La misma wishlist que la ultima orden temporal que ha realizado el usuario
                WishlistId = temporalOrder.WishlistId,
                UserId = userId,
                HashOrSession = hashOrSessionOrder
            };

            //Elimina el carrito si se ha hecho la compra con sesión iniciada           
            if (!temporalOrder.Quick)
            {
                ShoppingCart shoppingCart = await _unitOfWork.ShoppingCartRepository.GetIdShoppingCartByUserId(userId);
                if(shoppingCart != null)
                {
                    await _unitOfWork.CartContentRepository.DeleteByIdShoppingCartAsync(shoppingCart);
                }
            }

            //Order en la base de datos
            Order saveOrder = await _unitOfWork.OrderRepository.InsertAsync(order);

            /*saveOrder.Wishlist = temporalOrder.Wishlist;
            _unitOfWork.OrderRepository.Update(saveOrder);

            //Añade la orden a la lista de ordenes del usuario
            user.Orders.Add(saveOrder);
            _unitOfWork.UserRepository.Update(user);*/

            saveOrder.Wishlist = temporalOrder.Wishlist;

            _unitOfWork.TemporalOrderRepository.Delete(temporalOrder);

            await _unitOfWork.SaveAsync();

            return saveOrder;
        }

    }
}
