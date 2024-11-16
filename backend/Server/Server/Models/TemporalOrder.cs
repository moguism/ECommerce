using Server.DTOs;

namespace Server.Models
{
    public class TemporalOrder
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        /*Lista de productos y cantidad para poder almacenar tanto el carrito del usuario autenticado
         * Como el carrito almacenado en el Local Storage si no tiene sesión iniciada antes del pago
         * 
         */
        public IEnumerable<CartContentDto> CartContent { get; set; } = new List<CartContentDto>();

    }
}
