using Server.Mappers;
using Server.Models;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Server.Services
{
    public class EmailService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ProductMapper _productMapper;
        private readonly EmailSettings _emailSettings;
        public EmailService(UnitOfWork unitOfWork,ProductMapper productMapper, EmailSettings emailSettings) 
        {
            _unitOfWork = unitOfWork;
            _productMapper = productMapper;
            _emailSettings = emailSettings;
        }

        public  async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            using SmtpClient client = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailSettings.EmailFrom, _emailSettings.PasswordEmailFrom)
            };

            MailMessage mail = new MailMessage(_emailSettings.EmailFrom, to, subject, body)
            {
                IsBodyHtml = isHtml,
            };

            await client.SendMailAsync(mail);
        }
        public async Task CreateEmailUser(User user, Wishlist productsorder,int paymetId)
        {
            decimal totalprice = 0;
            string to = user.Email;
            string subject = "Envio de la compra realizada";
            StringBuilder body = new StringBuilder();
            body.AppendLine("<html> <h1>QUE BISHO GRACIAS POR COMPRAR</h1> <table><tr><td>Nombre</td><td>Imagen</td><td>Precio</td><td>Cantidad</td><td>Suma</td></tr>");
            foreach (ProductsToBuy products in productsorder.Products)
            {
                decimal price = 0;
                decimal totalpricequantity = 0;
                Product oneproduct = await _unitOfWork.ProductRepository.GetFullProductById(products.ProductId);
                price = oneproduct.Price / 100m;
                totalpricequantity = (products.Quantity * oneproduct.Price) / 100m;
                body.AppendLine($"<tr><td>{oneproduct.Name}</td><td><img src='{_productMapper.AddCorrectPath(oneproduct)}'></td><td>{price}€</td><td>{products.Quantity}</td><td>{totalpricequantity}€</td></tr>");
                totalprice += products.Quantity * oneproduct.Price;
            }
            body.AppendLine("</table></html>");
            body.AppendLine($"<h2>Su pedido ha costado: {totalprice / 100}€</h2>");
            if (paymetId == 1)
            {
                body.AppendLine($"<h3>Metodo de pago: Tarjeta</h3>");
            }
            else
            {
                body.AppendLine($"<h3>Metodo de pago: Etherium</h3>");
            }
            body.AppendLine($"<h4>Direccion de envio: {user.Address}</h4>");
            await SendEmailAsync(to, subject, body.ToString(), true);
        }
    }
}
