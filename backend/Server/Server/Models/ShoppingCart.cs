using Microsoft.EntityFrameworkCore;

namespace Server.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<CartContent> CartContent { get; set; } = new List<CartContent>();
        public ICollection<TemporalOrder> TemporalOrders { get; set; } = new List<TemporalOrder>();

        /* Temporal y Finished se eliminan para que no cree más carrito al usuario
         * Este solo puede tener 1 
         * Cuando se realiza el pago, se restan los productos que se han comprado del carrito 
        public bool Temporal { get; set; }
        public bool Finished { get; set; }
        */

    }
}
