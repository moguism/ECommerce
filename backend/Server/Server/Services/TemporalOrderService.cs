﻿using Server.DTOs;
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

        // ESTO Y LO SIGUIENTE CREO QUE ESTÁ DANDO POR CULO
        public async Task<Wishlist> CreateNewWishList(IEnumerable<CartContentDto> products)
        {
            Wishlist wishlist = new Wishlist();

            ProductsToBuyMapper productsToBuyMapper = new ProductsToBuyMapper();
            IList<ProductsToBuy> productsToBuyList = productsToBuyMapper.ToEntity(products).ToArray();

            // Asignar el Id de la wishlist a los productos después de guardar
            foreach (ProductsToBuy productToBuy in productsToBuyList)
            {
                // Asignamos correctamente el Id de la wishlist a cada producto
                Product product = await _unitOfWork.ProductRepository.GetByIdAsync(productToBuy.ProductId);
                if (product == null || product.Stock - productToBuy.Quantity < 0)
                {
                    return null;
                }
                productToBuy.ProductId = product.Id;
                productToBuy.PurchasePrice = product.Price;
                product.Stock -= productToBuy.Quantity;
                _unitOfWork.ProductRepository.Update(product);
            }

            wishlist.Products = productsToBuyList;

            await _unitOfWork.WishlistRepository.InsertAsync(wishlist);

            await _unitOfWork.SaveAsync();

            // Devolver la wishlist creada
            return wishlist;
        }

        public async Task<TemporalOrder> CreateTemporalOrder(User user, bool quick, TemporalOrderDto temporalOrderDto)
        {
            Wishlist wishlist = await CreateNewWishList(temporalOrderDto.CartContentDtos);// Añade a la nueva wislist los productos que el usuario quire comprar

            if (wishlist == null)
            {
                return null;
            }

            //La añade a la base de datos
            TemporalOrder order = await _unitOfWork.TemporalOrderRepository.InsertAsync(new TemporalOrder
            {
                UserId = user.Id,
                WishlistId = wishlist.Id,
                ExpirationDate = DateTime.UtcNow.AddMinutes(5),
                Quick = quick
            });


            //Resta el stock a los productos que quiere comprar el usuario
            /*foreach(ProductsToBuy productToBuy in wishlist.Products)
            {
                Product product = await _unitOfWork.ProductRepository.GetByIdAsync(productToBuy.ProductId);
                product.Stock -= productToBuy.Quantity;
                _unitOfWork.ProductRepository.Update(product);
            }*/

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

        public async Task<Order> CreateOrderFromTemporal(TemporalOrder temporalOrder, User user, int paymentType)
        {
            //Total price €
            long totalPriceCents = temporalOrder.Wishlist.Products.Sum(p => p.PurchasePrice * p.Quantity);
            decimal totalPriceEuros = totalPriceCents / 100;

            Order order = new Order
            {
                CreatedAt = DateTime.UtcNow,
                PaymentTypeId = paymentType,
                //La misma wishlist que la ultima orden temporal que ha realizado el usuario
                WishlistId = temporalOrder.WishlistId,
                UserId = user.Id,
                HashOrSession = temporalOrder.HashOrSession,
                Total = totalPriceCents,
            };

            //Elimina el carrito si se ha hecho la compra con sesión iniciada           
            if (!temporalOrder.Quick)
            {
                ShoppingCart shoppingCart = user.ShoppingCart;
                if (shoppingCart != null)
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

        public async Task<TemporalOrder> GetLastTemporalOrder(int userId)
        {

            return await _unitOfWork.TemporalOrderRepository.GetFullTemporalOrderByUserId(userId);
        }

        public async Task<User> GetMinimumUser(string stringId)
        {

            // Pilla el usuario de la base de datos
            return await _unitOfWork.UserRepository.GetByIdAsync(Int32.Parse(stringId));
        }

        public async Task<User> GetUserFromStringWithTemporal(string stringId)
        {

            // Pilla el usuario de la base de datos
            return await _unitOfWork.UserRepository.GetAllInfoWithTemporal(Int32.Parse(stringId));
        }

        public async Task<User> GetMinimumWithCart(string stringId)
        {

            // Pilla el usuario de la base de datos
            return await _unitOfWork.UserRepository.GetMinimumWithCart(Int32.Parse(stringId));
        }
    }
}